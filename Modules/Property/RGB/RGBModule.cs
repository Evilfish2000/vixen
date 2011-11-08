﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Property;
using Vixen.Module.Effect;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;
using Vixen.Sys;
using CommonElements.ColorManagement.ColorModels;
using System.Drawing;

namespace VixenModules.Property.RGB
{
	public class RGBModule : PropertyModuleInstanceBase
	{
		private RGBData _data;

		public override void SetDefaultValues()
		{
			List<ChannelNode> channels = new List<ChannelNode>(Owner.Children);
			if (channels.Count == 3) {
				_data.RGBType = RGBModelType.eIndividualRGBChannels;
				_data.RedChannelNode = channels[0].Id;
				_data.GreenChannelNode = channels[1].Id;
				_data.BlueChannelNode = channels[2].Id;
			} else {
				_data.RGBType = RGBModelType.eSingleRGBChannel;
			}
		}

		public override void Setup()
		{
			List<ChannelNode> channels = new List<ChannelNode>(Owner.Children);
			using (RGBSetup form = new RGBSetup(_data, channels.ToArray())) {
				form.ShowDialog();
			}
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as RGBData; }
		}

		public ChannelCommands RenderColorToCommands(Color color, Level level)
		{
			ChannelCommands result = new ChannelCommands();

			HSV finalColor = HSV.FromRGB(color);
			finalColor.V = level / 100.0;

			// if it's all getting dumped out each channel, just make 'smart' RGB commands for them
			if (_data.RGBType == RGBModelType.eSingleRGBChannel)
			{
				Color fullColorParameter = finalColor.ToRGB().ToArgb();
				result.AddCommandForChannel(Owner.Channel.Id, new Lighting.Polychrome.SetColor(fullColorParameter));
			}
			// otherwise, we're breaking it up by channel, so split the color up into discrete components
			else
			{
				// These need to be scaled: levels are 0-100, components are 0.0 - 1.0
				Level R = finalColor.ToRGB().R * 100.0;
				Level G = finalColor.ToRGB().G * 100.0;
				Level B = finalColor.ToRGB().B * 100.0;

				// populate the red channel(s) with a setlevel of the red value
				ChannelNode redNode = VixenSystem.Nodes.GetChannelNode(_data.RedChannelNode);
				if (redNode != null && redNode.Channel != null) {
					result.AddCommandForChannel(redNode.Channel.Id, new Lighting.Monochrome.SetLevel(R));
				}
				
				// populate the green channel(s) with a setlevel of the green value
				ChannelNode greenNode = VixenSystem.Nodes.GetChannelNode(_data.GreenChannelNode);
				if (greenNode != null && greenNode.Channel != null) {
					result.AddCommandForChannel(greenNode.Channel.Id, new Lighting.Monochrome.SetLevel(G));
				}
				
				// populate the blue channel(s) with a setlevel of the blue value
				ChannelNode blueNode = VixenSystem.Nodes.GetChannelNode(_data.BlueChannelNode);
				if (blueNode != null && blueNode.Channel != null) {
					result.AddCommandForChannel(blueNode.Channel.Id, new Lighting.Monochrome.SetLevel(B));
				}
			}

			return result;
		}

		public static ChannelCommands RenderColorToCommandsForNode(ChannelNode node, CommonElements.ColorManagement.ColorModels.RGB color, Level level)
		{
			RGBModule instance = node.Properties.Get(RGBDescriptor.ModuleID) as RGBModule;
			if (instance == null)
			{
				VixenSystem.Logging.Warning("Invalid attempt to render RGB color on node '" + node.Name + "', ID " + node.Id);
				return null;
			}

			return instance.RenderColorToCommands(color, level);
		}


		/// <summary>
		/// Given a ChannelNode, this method will find all child nodes that are ultimately renderable as either a monochrome or polychrome
		/// node. In effect, this means it will descend through all nodes, finding leaf nodes (no children) or nodes with the RGB property.
		/// </summary>
		/// <param name="node">The node to find all renderable children for.</param>
		/// <returns>An enumerable of all renderable children.</returns>
		public static IEnumerable<ChannelNode> FindAllRenderableChildren(ChannelNode node)
		{
			HashSet<ChannelNode> result = new HashSet<ChannelNode>();
			if (node.Properties.Contains(RGBDescriptor.ModuleID)) {
				result.Add(node);
			} else {
				if (node.IsLeaf) {
					result.Add(node);
				} else {
					foreach (ChannelNode child in node.Children)
						result.AddRange(FindAllRenderableChildren(node));
				}
			}

			return result;
		}

		/// <summary>
		/// Given ChannelNodes, this method will find all child nodes that are ultimately renderable as either a monochrome or polychrome
		/// nodes. In effect, this means it will descend through all nodes, finding leaf nodes (no children) or nodes with the RGB property.
		/// </summary>
		public static IEnumerable<ChannelNode> FindAllRenderableChildren(IEnumerable<ChannelNode> nodes)
		{
			HashSet<ChannelNode> result = new HashSet<ChannelNode>();
			foreach (ChannelNode node in nodes) {
				result.AddRange(FindAllRenderableChildren(node));
			}
			return result;
		}


		#region Reverse-mapping of commands to a channelNode and color

		public static Dictionary<ChannelNode, Color> MapChannelCommandsToColors(Dictionary<Channel, Command> channelsCommands)
		{
			Dictionary<ChannelNode, Color> result = new Dictionary<ChannelNode,Color>();

			// go through all the channel/command mappings in the given data, and do our best to map them back to something sane.
			foreach (KeyValuePair<Channel, Command> kvp in channelsCommands) {
				Channel channel = kvp.Key;
				Command command = kvp.Value;
				bool success = false;			// if we've successfully found something for this channel/command.

				ChannelNode node = VixenSystem.Channels.GetChannelNodeForChannel(channel);
				if (node == null) {
					VixenSystem.Logging.Warning("RGB Mapping: can't find the reverse ChannelNode mapping for channel: " + channel.Name);
					continue;
				}

				// if the node for this channel has the RGB property, then it must be configured as a 'smart' RGB channel.
				// Double check that it is, and if so, find any Polychrome commands and figure out colors for the result.
				if (node.Properties.Contains(RGBDescriptor.ModuleID)) {
					if (((node.Properties.Get(RGBDescriptor.ModuleID) as RGBModule).ModuleData as RGBData).RGBType != RGBModelType.eSingleRGBChannel) {
						VixenSystem.Logging.Warning("RGB Mapping: got a channel that has an RGB property, but not with the SingleRGB type, that has a command.");
					}

					if (command is Vixen.Commands.Lighting.Polychrome.SetColor) {
						if (result.ContainsKey(node)) {
							VixenSystem.Logging.Warning("RGB Mapping: got an RGB channel with a Polychrome.Setcolor, but there's already a result for this channelNode.");
						}
						result[node] = (command as Vixen.Commands.Lighting.Polychrome.SetColor).Color;
						success = true;
					} else {
						VixenSystem.Logging.Warning("RGB Mapping: got a channel that has an RGB property, but was given a command that wasn't Polychrome.Setcolor.");
					}
				}
				// the node doesn't have an RGB property, so maybe it's a individual RGB channel for a parent node. Check all this node's parents,
				// and see if any have RGB properties; if so, map this channel's color to the respective component on the parent.
				else {
					foreach (ChannelNode parent in node.Parents) {
						if (parent.Properties.Contains(RGBDescriptor.ModuleID)) {
							RGBData parentData = (parent.Properties.Get(RGBDescriptor.ModuleID) as RGBModule).ModuleData as RGBData;
							if (parentData.RGBType != RGBModelType.eIndividualRGBChannels) {
								VixenSystem.Logging.Warning("RGB Mapping: got a channel that has a parent with the RGB property that isn't IndividualChannels.");
							}

							if (command is Vixen.Commands.Lighting.Monochrome.SetLevel) {
								bool setRed, setGreen, setBlue;
								byte colorLevel = (byte)(((command as Vixen.Commands.Lighting.Monochrome.SetLevel).Level / 100.0) * Byte.MaxValue);

								setRed = (parentData.RedChannelNode == node.Id);
								setGreen = (parentData.GreenChannelNode == node.Id);
								setBlue = (parentData.BlueChannelNode == node.Id);

								if (setRed || setGreen || setBlue) {
									Color color;
									if (result.ContainsKey(parent))
										color = result[parent];
									else
										color = Color.FromArgb(0, 0, 0);

									if (setRed)   color = Color.FromArgb(colorLevel, color.G, color.B);
									if (setGreen) color = Color.FromArgb(color.R, colorLevel, color.B);
									if (setBlue)  color = Color.FromArgb(color.R, color.G, colorLevel);

									result[parent] = color;
									success = true;
								}
							} else {
								VixenSystem.Logging.Warning("RGB Mapping: got a channel with a parent that has an RGB individual " +
									"components property, but was given a command that wasn't Monochrome.Setcolor.");
							}
						}
					}
				}

				// if we haven't found a color for either of the above cases, just assume it's a monochrome, and map it back to
				// to a white color if it's a setlevel command (and if we don't already have a result for it!).
				if (!success) {
					if (command is Vixen.Commands.Lighting.Monochrome.SetLevel) {
						if (result.ContainsKey(node))
							continue;

						byte colorLevel = (byte)(((command as Vixen.Commands.Lighting.Monochrome.SetLevel).Level / 100.0) * Byte.MaxValue);
						result[node] = Color.FromArgb(colorLevel, colorLevel, colorLevel);
					}
				}
			}

			return result;
		}

		public static List<ChannelNode> GetVisuallyRenderableChildNodes(ChannelNode[] targetNodes)
		{
			List<ChannelNode> result = new List<ChannelNode>();

			foreach (ChannelNode targetNode in targetNodes) {
				if (targetNode.Properties.Contains(RGBDescriptor.ModuleID)) {
					result.Add(targetNode);
				} else if (targetNode.Children.Count() > 0) {
					result.AddRange(GetVisuallyRenderableChildNodes(targetNode.Children.ToArray()));
				} else {
					result.Add(targetNode);
				}
			}

			return result;
		}

		#endregion
	}
}
