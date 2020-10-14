using SignUp.Entities;
using SignUp.Messaging;
using SignUp.Messaging.Messages.Events;
using System;

namespace SignUp.Web.ProspectSave
{
    public class AsynchronousProspectSaveHandler : IProspectSaveHandler
    {
        private readonly MessageQueue _queue;

        public AsynchronousProspectSaveHandler(MessageQueue queue)
        {
            _queue = queue;
        }

        public void SaveProspect(Prospect prospect)
        {
            var eventMessage = new ProspectSignedUpEvent
            {
                Prospect = prospect,
                SignedUpAt = DateTime.UtcNow
            };

            _queue.Publish(eventMessage);
        }
    }
}