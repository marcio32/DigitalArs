﻿using AutoMapper;
using DigitalArs_copia.DataAccess.Repositories.Interfaces;
using DigitalArs_copia.DTO_s;
using DigitalArs_copia.Entities;
using DigitalArs_copia.Helper;
using Microsoft.EntityFrameworkCore;

namespace DigitalArs_copia.DataAccess.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly IMapper _mapper;

        public UserRepository(ContextDB contextDB, IMapper mapper) : base(contextDB)
        {
            _mapper = mapper;
        }

        public async Task<bool> UpdateUser(UserRegisterDTO userRegisterDTO, int id, int parameter)
        {
            try
            {

                var userFinding = await GetById(id);

                if (userFinding == null)
                {

                    return false;

                }
                if (parameter == 0)
                {

                    var user = _mapper.Map<User>(userRegisterDTO);
                    _mapper.Map(user, userFinding);
                    _contextDB.Update(userFinding);
                    return true;

                }
                if (parameter == 1)
                {
                   
                    _contextDB.Update(userFinding);
                    return true;

                }

                return false;
            }
            catch (Exception)
            {

                return false;
            }
        }


        public virtual async Task<List<UserDTO>> GetAllUsers(int parameter)
        {
            try
            {
                if (parameter == 0)
                {
                    List<User> users = await _contextDB.Users
                        .Include(user => user.Role)
                        .Where(user => user.IsActive)
                        .ToListAsync();

                    return _mapper.Map<List<UserDTO>>(users);
                }
                if(parameter == 1)
                {
                    List<User> users = await _contextDB.Users
                        .Include(user => user.Role)
                        .Where(user => user.IsActive == false)
                        .ToListAsync();

                    return _mapper.Map<List<UserDTO>>(users);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }


        public async Task<UserDTO> GetUserById(int id, int parameter)
        {
            try
            {
                User userFinding = await _contextDB.Users
                           .Include(u => u.Role)
                           .Where(u => u.Id == id)
                           .FirstOrDefaultAsync();

                if (userFinding == null)
                {
                    return null;
                }

                if ( parameter == 0)
                {

                    return _mapper.Map<UserDTO>(userFinding);
                }
                return null;

            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> DeleteUserById(int id, int parameter)
        {

            try
            {
                User userFinding = await base.GetById(id);

                if (userFinding == null)
                {
                    return false;
                }

                if (userFinding != null && parameter == 0)
                {
                    userFinding.IsActive = false;
                    await _contextDB.SaveChangesAsync();
                    return true;
                }
                if (userFinding != null && parameter == 1)
                {
                    _contextDB.Users.Remove(userFinding);
                    await _contextDB.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public virtual async Task<bool> InsertUser(UserRegisterDTO userRegisterDTO)
        {
            try
            {
                if(userRegisterDTO.RoleId.HasValue && userRegisterDTO.RoleId != 3)
                {
                    return false;
                }

                var user = _mapper.Map<User>(userRegisterDTO);
                user.Password = PasswordEncryptHelper.EncryptPassword(user.Password, user.Email);
                var response = await base.Insert(user);
                return response;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public async Task<User?> AuthenticateCredentials(AuthenticateDTO dto)
        {

            try
            {
                return await _contextDB.Users.Include(user => user.Role).SingleOrDefaultAsync
                              (user => user.Email == dto.Email && user.Password == PasswordEncryptHelper.EncryptPassword(dto.Password, dto.Email));
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
