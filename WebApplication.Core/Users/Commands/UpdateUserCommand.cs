﻿using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using WebApplication.Core.Common.Exceptions;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Commands
{
    public class UpdateUserCommand : IRequest<UserDto>
    {
        public int Id { get; set; }
        public string GivenNames { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        public class Validator : AbstractValidator<UpdateUserCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                    .GreaterThan(0)
                    .WithMessage("'Id' must be greater than '0'.");

                RuleFor(x => x.GivenNames)
                    .NotEmpty()
                    .WithMessage("'Given Names' must not be empty.");


                RuleFor(x => x.LastName)
                    .NotEmpty()
                    .WithMessage("'Last Name' must not be empty.");

                RuleFor(x => x.EmailAddress)
                    .NotEmpty()
                    .WithMessage("'Email Address' must not be empty.");

                RuleFor(x => x.MobileNumber)
                    .NotEmpty()
                    .WithMessage("'Mobile Number' must not be empty.");
            }
        }

        public class Handler : IRequestHandler<UpdateUserCommand, UserDto>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                User userToUpdate = _mapper.Map<User>(request);
                
                User? updatedUser = await _userService.UpdateAsync(userToUpdate, cancellationToken);
                if (updatedUser == null)
                {
                    throw new NotFoundException($"The user '{request.Id}' could not be found.");
                }

                return _mapper.Map<UserDto>(updatedUser);
            }
        }
    }
}
