﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using NetDaemon.Model3.Common;

namespace NetDaemon.Model3.Entities
{
    public record EntityState
    {
        public string EntityId { get; init; } = "";
    
        public string? State { get; init; }

        internal JsonElement? AttributesJson { get; init; }
        public virtual object? Attributes => AttributesJson?.ToObject<Dictionary<string, object>>() ?? new Dictionary<string, object>();
    
        public DateTime? LastChanged { get; init; }
    
        public DateTime? LastUpdated { get; init; }
    
        public Context? Context { get; init; }
    }
    
    public record EntityState<TState, TAttributes> : EntityState 
        where TAttributes : class
    {
        private readonly Lazy<TAttributes?> _attributesLazy;

        public EntityState(EntityState source) : base(source)
        {
            _attributesLazy = new (() => AttributesJson?.ToObject<TAttributes>() ?? default);            
        }

        public new TState? State => base.State == null ? default : (TState?)Convert.ChangeType(base.State, typeof(TState), CultureInfo.InvariantCulture);
        public override TAttributes? Attributes => _attributesLazy.Value;
    }
}