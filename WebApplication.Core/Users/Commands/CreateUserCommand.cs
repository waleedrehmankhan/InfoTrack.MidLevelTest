using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using log4net;
using MediatR;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Commands
{
    public class CreateUserCommand : IRequest<UserDto>
    {
        public string GivenNames { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        public class Validator : AbstractValidator<CreateUserCommand>
        {
            public Validator()
            {
                RuleFor(x => x.GivenNames)
                    .NotEmpty();

                RuleFor(x => x.LastName)
                    .NotEmpty();

                RuleFor(x => x.EmailAddress)
                    .NotEmpty();

                RuleFor(x => x.MobileNumber)
                    .NotEmpty();
            }
        }

        public class Handler : IRequestHandler<CreateUserCommand, UserDto>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;
            private static readonly ILog _log = LogManager.GetLogger(typeof(CreateUserCommand));

            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
            {
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    User userToAdd = new User
                    {
                        GivenNames = request.GivenNames,
                        LastName = request.LastName,
                        ContactDetail = new ContactDetail
                        {
                            EmailAddress = request.EmailAddress,
                            MobileNumber = request.MobileNumber
                        }
                    };

                    User addedUser = await _userService.AddAsync(userToAdd, cancellationToken);
                    UserDto result = _mapper.Map<UserDto>(addedUser);

                    return result;
                }
                finally
                {
                    stopwatch.Stop();
                    if (_log.IsInfoEnabled)
                    {
                        _log.Info($"{nameof(CreateUserCommand)} Handler execution time: {stopwatch.Elapsed.TotalMilliseconds} ms");
                    }
                }
            }
        }
    }
}
