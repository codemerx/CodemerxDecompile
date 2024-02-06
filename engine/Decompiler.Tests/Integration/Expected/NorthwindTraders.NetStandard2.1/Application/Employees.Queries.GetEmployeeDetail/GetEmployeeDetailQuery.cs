using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Interfaces;
using Northwind.Domain.Entities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Employees.Queries.GetEmployeeDetail
{
	public class GetEmployeeDetailQuery : IRequest<EmployeeDetailVm>, IBaseRequest
	{
		public int Id
		{
			get;
			set;
		}

		public GetEmployeeDetailQuery()
		{
		}

		public class GetEmployeeDetailQueryHandler : IRequestHandler<GetEmployeeDetailQuery, EmployeeDetailVm>
		{
			private readonly INorthwindDbContext _context;

			private readonly IMapper _mapper;

			public GetEmployeeDetailQueryHandler(INorthwindDbContext context, IMapper mapper)
			{
				this._context = context;
				this._mapper = mapper;
			}

			public async Task<EmployeeDetailVm> Handle(GetEmployeeDetailQuery request, CancellationToken cancellationToken)
			{
				GetEmployeeDetailQuery.GetEmployeeDetailQueryHandler.u003cu003ec__DisplayClass3_0 variable = null;
				DbSet<Employee> employees = this._context.Employees;
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Employee), "e");
				BinaryExpression binaryExpression = Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Employee).GetMethod("get_EmployeeId").MethodHandle)), Expression.Property(Expression.Field(Expression.Constant(variable, typeof(GetEmployeeDetailQuery.GetEmployeeDetailQueryHandler.u003cu003ec__DisplayClass3_0)), FieldInfo.GetFieldFromHandle(typeof(GetEmployeeDetailQuery.GetEmployeeDetailQueryHandler.u003cu003ec__DisplayClass3_0).GetField("request").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(GetEmployeeDetailQuery).GetMethod("get_Id").MethodHandle)));
				ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
				EmployeeDetailVm employeeDetailVm = await EntityFrameworkQueryableExtensions.SingleOrDefaultAsync<EmployeeDetailVm>(Extensions.ProjectTo<EmployeeDetailVm>(employees.Where<Employee>(Expression.Lambda<Func<Employee, bool>>(binaryExpression, parameterExpressionArray)), this._mapper.get_ConfigurationProvider(), Array.Empty<Expression<Func<EmployeeDetailVm, object>>>()), cancellationToken);
				EmployeeDetailVm employeeDetailVm1 = employeeDetailVm;
				employeeDetailVm = null;
				EmployeeDetailVm employeeDetailVm2 = employeeDetailVm1;
				variable = null;
				employeeDetailVm1 = null;
				return employeeDetailVm2;
			}
		}
	}
}