using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data {
  public class UserRepository: IUserRepository {
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    public UserRepository(DataContext context, IMapper mapper) {
      _mapper = mapper;
      _context = context;
    }

    public async Task<MemberDto> GetMemberAsync(string username) {
      return await _context.Users
        .Where(x=> x.UserName ==  username)
        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }

    public async Task<PageList<MemberDto>> GetMembersAsync(UserParams userParam) {
      
      var query =  _context.Users.AsQueryable();
       query = query.Where(u => u.UserName != userParam.CurrentUsername);
      query = query.Where(u => u.Gender == userParam.Gender);

      var minDob =DateTime.Today.AddYears(-userParam.MaxAge - 1);
      var maxDob =DateTime.Today.AddYears(-userParam.MinAge - 1);

      query =  query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

      query = userParam.OrderBy switch
      {
        "created" => query.OrderByDescending(u => u.Created),
        _ => query.OrderByDescending(u => u.LastActive)
      };

      return await PageList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(
          _mapper.ConfigurationProvider).AsNoTracking(),
            userParam.PageNumber, userParam.PageSize);
    }

    public async Task<AppUser> GetUserByIdAsync(int id) {
      return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username) {
      return await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync() {
      return await _context.Users.Include(p => p.Photos).ToListAsync();
    }

    public async Task<bool> SaveAllAsync() {
      return await _context.SaveChangesAsync() > 0;
    }

    public void Update(AppUser user) {
      _context.Entry(user).State = EntityState.Modified;
    }
  }
}