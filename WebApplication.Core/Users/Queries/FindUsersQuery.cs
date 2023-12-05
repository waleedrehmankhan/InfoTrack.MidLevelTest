using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using log4net;
using MediatR;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Queries
{
    public class FindUsersQuery : IRequest<IEnumerable<UserDto>>
    {
        public string? GivenNames { get; set; }
        public string? LastName { get; set; }

        public class Validator : AbstractValidator<FindUsersQuery>
        {
            public Validator()
            {
                RuleFor(x => x.GivenNames)
                    .NotEmpty()
                    .When(x => string.IsNullOrWhiteSpace(x.LastName));

                RuleFor(x => x.LastName)
                    .NotEmpty()
                    .When(x => string.IsNullOrWhiteSpace(x.GivenNames));
            }
        }

        public class Handler : IRequestHandler<FindUsersQuery, IEnumerable<UserDto>>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;
            private static readonly ILog _log = LogManager.GetLogger(typeof(FindUsersQuery));

            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            public async Task<IEnumerable<UserDto>> Handle(FindUsersQuery request, CancellationToken cancellationToken)
            {
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    IEnumerable<User> users = await _userService.FindAsync(request.GivenNames, request.LastName, cancellationToken);
                    return users.Select(user => _mapper.Map<UserDto>(user));
                }
                finally
                {
                    stopwatch.Stop();
                    if (_log.IsInfoEnabled)
                    {
                        _log.Info($"{nameof(FindUsersQuery)} Handler execution time: {stopwatch.Elapsed.TotalMilliseconds} ms");
                    }
                }
            }
        }
    }
}
