using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using log4net;
using MediatR;
using WebApplication.Core.Common.Exceptions;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Queries
{
    public class GetUserQuery : IRequest<UserDto>
    {
        public int Id { get; set; }

        public class Validator : AbstractValidator<GetUserQuery>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                    .GreaterThan(0);
            }
        }

        public class Handler : IRequestHandler<GetUserQuery, UserDto>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;
            private static readonly ILog _log = LogManager.GetLogger(typeof(GetUserQuery));

            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
            {
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    User? user = await _userService.GetAsync(request.Id, cancellationToken);

                    if (user is default(User)) throw new NotFoundException($"The user '{request.Id}' could not be found.");

                    UserDto result = _mapper.Map<UserDto>(user);

                    return result;
                }
                finally
                {
                    stopwatch.Stop();
                    if (_log.IsInfoEnabled)
                    {
                        _log.Info($"{nameof(GetUserQuery)} Handler execution time: {stopwatch.Elapsed.TotalMilliseconds} ms");
                    }
                }
            }
        }
    }
}
