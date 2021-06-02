using System;
using NServiceBus;

namespace Shared
{
    public class Ping : ICommand
    {
        public DateTimeOffset IssuedOn { get; set; }
    }
}