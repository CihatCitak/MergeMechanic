using System;
using System.Collections.Generic;
using Game.Core.Events;
using NUnit.Framework;

namespace Game.Tests
{
    public class EventBusTests
    {
        private readonly struct TestEvent : IEvent
        {
            public readonly int Value;

            public TestEvent(int value)
            {
                Value = value;
            }
        }

        private readonly List<Action<TestEvent>> _subscribed = new();

        [TearDown]
        public void TearDown()
        {
            foreach (var handler in _subscribed)
                EventBus.Unsubscribe(handler);

            _subscribed.Clear();
        }

        private void Subscribe(Action<TestEvent> handler)
        {
            _subscribed.Add(handler);
            EventBus.Subscribe(handler);
        }

        [Test]
        public void Fire_CallsEverySubscriberWithEventData()
        {
            var received = new List<int>();
            Subscribe(e => received.Add(e.Value));
            Subscribe(e => received.Add(e.Value));

            EventBus.Fire(new TestEvent(42));

            CollectionAssert.AreEqual(new[] { 42, 42 }, received);
        }

        [Test]
        public void Fire_AfterUnsubscribe_DoesNotCallHandler()
        {
            var calls = 0;
            Action<TestEvent> handler = _ => calls++;
            Subscribe(handler);

            EventBus.Unsubscribe(handler);
            EventBus.Fire(new TestEvent());

            Assert.AreEqual(0, calls);
        }

        [Test]
        public void Fire_HandlerUnsubscribesItself_OtherHandlersStillCalled()
        {
            var otherCalls = 0;
            Action<TestEvent> selfRemoving = null;
            selfRemoving = _ => EventBus.Unsubscribe(selfRemoving);

            Subscribe(_ => otherCalls++);
            Subscribe(selfRemoving);
            Subscribe(_ => otherCalls++);

            EventBus.Fire(new TestEvent());

            Assert.AreEqual(2, otherCalls);
        }

        [Test]
        public void Fire_HandlerThrows_OtherHandlersStillCalled()
        {
            var otherCalls = 0;
            Subscribe(_ => otherCalls++);
            Subscribe(_ => throw new InvalidOperationException());
            Subscribe(_ => otherCalls++);

            LogUtility.WithoutLogs(() => EventBus.Fire(new TestEvent()));

            Assert.AreEqual(2, otherCalls);
        }
    }
}
