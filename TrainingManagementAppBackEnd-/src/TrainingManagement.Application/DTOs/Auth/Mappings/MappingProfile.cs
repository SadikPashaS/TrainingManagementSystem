using AutoMapper;
using TrainingManagement.Application.DTOs.Auth;
using TrainingManagement.Application.DTOs.Dashboard;
using TrainingManagement.Application.DTOs.Training;
using TrainingManagement.Application.DTOs.User;
using TrainingManagement.Application.DTOs.UserTraining;
using TrainingManagement.Domain.Entities;
using System;

namespace TrainingManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
        
        CreateMap<User, AuthResponseDto>()
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.TokenExpiry, opt => opt.Ignore());

        // Training mappings
        CreateMap<Training, TrainingDto>();
        CreateMap<CreateTrainingDto, Training>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
        CreateMap<UpdateTrainingDto, Training>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // UserTraining mappings
        CreateMap<UserTraining, UserTrainingDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.TrainingName, opt => opt.MapFrom(src => src.Training.TrainingName));
        
        CreateMap<AssignTrainingDto, UserTraining>()
            .ForMember(dest => dest.AssignedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.TrainingStatus.Pending));

        // Dashboard mappings
        CreateMap<UserTraining, UserTrainingDto>();
    }
}