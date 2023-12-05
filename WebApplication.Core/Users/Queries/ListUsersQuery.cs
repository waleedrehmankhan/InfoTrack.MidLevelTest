using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using log4net;
using MediatR;
using WebApplication.Core.Common.Exceptions;
using WebApplication.Core.Common.Models;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Queries
{
    public class ListUsersQuery : IRequest<PaginatedDto<IEnumerable<UserDto>>>
    {
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; } = 10;

        public class Validator : AbstractValidator<ListUsersQuery>
        {
            public Validator()
            {
                RuleFor(query => query.PageNumber)
                    .GreaterThan(0)
                    .WithMessage("'Page Number' must be greater than '0'.");
            }
        }

        public class Handler : IRequestHandler<ListUsersQuery, PaginatedDto<IEnumerable<UserDto>>>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;
            private static readonly ILog _log = LogManager.GetLogger(typeof(ListUsersQuery));

            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            public async Task<PaginatedDto<IEnumerable<UserDto>>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
            {
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    IEnumerable<User> paginatedUsers = await _userService.GetPaginatedAsync(request.PageNumber, request.ItemsPerPage, cancellationToken);
                    if (paginatedUsers == null || !paginatedUsers.Any())
                    {
                        throw new NotFoundException($"No users found.");
                    }

                    IEnumerable<UserDto> userDtos = _mapper.Map<IEnumerable<UserDto>>(paginatedUsers);
                    bool hasNextPage = (request.PageNumber * request.ItemsPerPage) == paginatedUsers.Count();

                    return new PaginatedDto<IEnumerable<UserDto>>
                    {
                        Data = userDtos,
                        HasNextPage = hasNextPage
                    };
                }
                finally
                {
                    stopwatch.Stop();
                    if (_log.IsInfoEnabled)
                    {
                        _log.Info($"{nameof(ListUsersQuery)} Handler execution time: {stopwatch.Elapsed.TotalMilliseconds} ms");
                    }
                }
            }
        }
    }
}
