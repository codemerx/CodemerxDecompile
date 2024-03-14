using MediatR;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Exceptions;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Models;
using Northwind.Domain.Entities;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Employees.Commands.DeleteEmployee
{
	public class DeleteEmployeeCommand : IRequest, IRequest<Unit>, IBaseRequest
	{
		public int Id
		{
			get;
			set;
		}

		public DeleteEmployeeCommand()
		{
		}

		public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand>, IRequestHandler<DeleteEmployeeCommand, Unit>
		{
			private readonly INorthwindDbContext _context;

			private readonly IUserManager _userManager;

			private readonly ICurrentUserService _currentUser;

			public DeleteEmployeeCommandHandler(INorthwindDbContext context, IUserManager userManager, ICurrentUserService currentUser)
			{
				this._context = context;
				this._userManager = userManager;
				this._currentUser = currentUser;
			}

			public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
			{
				DbSet<Employee> employees = this._context.Employees;
				Object[] id = new Object[] { request.Id };
				Employee employee = await employees.FindAsync(id);
				Employee employee1 = employee;
				employee = null;
				if (employee1 == null)
				{
					throw new NotFoundException("Employee", (object)request.Id);
				}
				if (employee1.get_UserId() == this._currentUser.UserId)
				{
					throw new BadRequestException("Employees cannot delete their own account.");
				}
				if ((object)employee1.get_UserId() != (object)null)
				{
					await this._userManager.DeleteUserAsync(employee1.get_UserId());
				}
				this._context.Employees.Remove(employee1);
				await this._context.SaveChangesAsync(cancellationToken);
				Unit value = Unit.Value;
				employee1 = null;
				return value;
			}
		}
	}
}