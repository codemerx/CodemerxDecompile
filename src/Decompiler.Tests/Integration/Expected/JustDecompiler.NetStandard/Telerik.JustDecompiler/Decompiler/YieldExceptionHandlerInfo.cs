using Mono.Cecil;
using System;
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

		public FieldReference DisposableField
		{
			get
			{
				return this.disposableField;
			}
		}

		public FieldReference EnumeratorField
		{
			get
			{
				return this.enumeratorField;
			}
		}

		public MethodDefinition FinallyMethodDefinition
		{
			get
			{
				return this.finallyMethodDef;
			}
		}

		public YieldExceptionHandlerType HandlerType
		{
			get
			{
				return this.handlerType;
			}
		}

		public int NextState
		{
			get
			{
				return this.nextState;
			}
		}

		public HashSet<int> TryStates
		{
			get
			{
				return this.tryStates;
			}
		}

		private YieldExceptionHandlerInfo(HashSet<int> tryStates)
		{
			this.tryStates = tryStates;
			this.handlerType = YieldExceptionHandlerType.Method;
			this.finallyMethodDef = null;
			this.nextState = -1;
			this.enumeratorField = null;
			this.disposableField = null;
		}

		public YieldExceptionHandlerInfo(HashSet<int> tryStates, MethodDefinition finallyMethodDef) : this(tryStates)
		{
			this.handlerType = YieldExceptionHandlerType.Method;
			this.finallyMethodDef = finallyMethodDef;
		}

		public YieldExceptionHandlerInfo(HashSet<int> tryStates, int nextState, FieldReference enumeratorField, FieldReference disposableField) : this(tryStates)
		{
			this.handlerType = (enumeratorField == null ? YieldExceptionHandlerType.SimpleConditionalDispose : YieldExceptionHandlerType.ConditionalDispose);
			this.nextState = nextState;
			this.enumeratorField = enumeratorField;
			this.disposableField = disposableField;
		}

		public int CompareTo(YieldExceptionHandlerInfo other)
		{
			if (this.TryStates.SetEquals(other.TryStates))
			{
				return 0;
			}
			if (this.TryStates.IsProperSupersetOf(other.TryStates))
			{
				return 1;
			}
			return -1;
		}
	}
}