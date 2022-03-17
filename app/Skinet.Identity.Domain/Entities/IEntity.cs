using System;

namespace Skinet.Identity.Domain.Entities
{
    public interface IEntity<TId> where TId : IComparable, IEquatable<TId>
    {
        TId Id { get; }
    }
}