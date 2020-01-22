using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Domain.Models
{
    public interface IEntityWithTypedId<TId>
    {
        TId Id { get; }
    }
}
