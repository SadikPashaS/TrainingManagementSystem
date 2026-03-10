using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TrainingManagement.Application.DTOs.User;
using TrainingManagement.Application.Interfaces;
using TrainingManagement.Domain.Entities;
using TrainingManagement.Domain.Interfaces;

namespace TrainingManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        if (await UserExistsAsync(createUserDto.UserId))
        {
            throw new InvalidOperationException($"User with ID {createUserDto.UserId} already exists");
        }

        var user = _mapper.Map<User>(createUserDto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        user.CreatedAt = DateTime.UtcNow;

        var createdUser = await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<UserDto>(createdUser);
    }

    public async Task UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        _mapper.Map(updateUserDto, user);
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        // Soft delete
        user.IsActive = false;
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<bool> UserExistsAsync(string userId)
    {
        return await _unitOfWork.Users.ExistsAsync(u => u.UserId == userId);
    }
}