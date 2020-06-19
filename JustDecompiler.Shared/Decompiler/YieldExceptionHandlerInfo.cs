using System;
using Mono.Cecil;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler
{
    internal struct YieldExceptionHandlerInfo : IComparable<YieldExceptionHandlerInfo>
    {
        private readonly HashSet<int> tryStates;
        private readonly YieldExceptionHandlerType handlerType;
        private readonly MethodDefinition finallyMethodDef;
        private readonly int nextState;
        private readonly FieldReference enumeratorField;
        private readonly FieldReference disposableField;

        /// <summary>
        /// Gets the states which the try handles.
        /// </summary>
        public HashSet<int> TryStates
        {
            get
            {
                return this.tryStates;
            }
        }

        /// <summary>
        /// Gets the type of the finally handler.
        /// </summary>
        public YieldExceptionHandlerType HandlerType
        {
            get
            {
                return this.handlerType;
            }
        }

        /// <summary>
        /// Gets the method in which resides the implementation of the finally block of the try/finally construct.
        /// </summary>
        public MethodDefinition FinallyMethodDefinition
        {
            get
            {
                return this.finallyMethodDef;
            }
        }

        /// <summary>
        /// Gets the value used for setting the state field.
        /// </summary>
        public int NextState
        {
            get
            {
                return this.nextState;
            }
        }

        /// <summary>
        /// Gets the enumerator field.
        /// </summary>
        public FieldReference EnumeratorField
        {
            get
            {
                return this.enumeratorField;
            }
        }

        /// <summary>
        /// Gets the disposable field.
        /// </summary>
        public FieldReference DisposableField
        {
            get
            {
                return this.disposableField;
            }
        }

        private YieldExceptionHandlerInfo(HashSet<int> tryStates)
        {
            this.tryStates = tryStates;
            this.handlerType = default(YieldExceptionHandlerType);
            this.finallyMethodDef = null;
            this.nextState = -1;
            this.enumeratorField = null;
            this.disposableField = null;
        }

        public YieldExceptionHandlerInfo(HashSet<int> tryStates, MethodDefinition finallyMethodDef)
            : this(tryStates)
        {
            this.handlerType = YieldExceptionHandlerType.Method;
            this.finallyMethodDef = finallyMethodDef;
        }

        public YieldExceptionHandlerInfo(HashSet<int> tryStates, int nextState, FieldReference enumeratorField, FieldReference disposableField)
            : this(tryStates)
        {
            this.handlerType = enumeratorField == null ? YieldExceptionHandlerType.SimpleConditionalDispose : YieldExceptionHandlerType.ConditionalDispose;
            this.nextState = nextState;
            this.enumeratorField = enumeratorField;
            this.disposableField = disposableField;
        }

        public int CompareTo(YieldExceptionHandlerInfo other)
        {
            if(this.TryStates.SetEquals(other.TryStates))
            {
                return 0;
            }

            if (this.TryStates.IsProperSupersetOf(other.TryStates))
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
