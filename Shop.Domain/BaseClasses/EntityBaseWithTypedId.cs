using System;
using System.Collections.Generic;
using System.Text;
using Shop.Domain.Models;

namespace Shop.Domain.Models
{
    public abstract class EntityBaseWithTypedId<TId> : ValidatableObject, IEntityWithTypedId<TId>
    {
        public virtual TId Id { get; protected set; }
    }
}
