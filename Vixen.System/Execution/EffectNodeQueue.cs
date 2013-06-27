﻿using System;
using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.Execution
{
	internal class EffectNodeQueue : IDisposable
	{
		private Queue<IEffectNode> _queue;
		//private ConcurrentQueue<IEffectNode> _queue;

		public EffectNodeQueue()
		{
			_queue = new Queue<IEffectNode>();
			//_queue = new ConcurrentQueue<IEffectNode>();
		}

		public EffectNodeQueue(IEnumerable<IEffectNode> items)
		{
			_queue = new Queue<IEffectNode>(items);
			//_queue = new ConcurrentQueue<IEffectNode>(items);
		}

		public void Add(IEffectNode item)
		{
			_queue.Enqueue(item);
		}

		public IEnumerable<IEffectNode> Get(TimeSpan time)
		{
			IEffectNode effectNode;
			do {
				effectNode = null;
				if (_queue.Count <= 0) continue;

				effectNode = _queue.Peek();
				effectNode = (time >= effectNode.StartTime) ? _queue.Dequeue() : null;

				//if(_queue.TryPeek(out effectNode)) {
				//    if(time >= effectNode.StartTime) {
				//        _queue.TryDequeue(out effectNode);
				//    } else {
				//        effectNode = null;
				//    }
				//}

				if (effectNode != null) yield return effectNode;
			} while (effectNode != null);
		}

		public void Dispose()
		{
			_queue.Clear();
			_queue = null;
			GC.SuppressFinalize(this);
		}
	}
}