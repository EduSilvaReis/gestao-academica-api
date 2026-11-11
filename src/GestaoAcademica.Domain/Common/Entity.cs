using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoAcademica.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; protected init; } = Guid.CreateVersion7();
}
