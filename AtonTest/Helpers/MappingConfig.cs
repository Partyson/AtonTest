using AtonTest.Dto;
using AtonTest.Entities;
using Mapster;
using JetBrains.Annotations;

namespace AtonTest.Helpers;

[UsedImplicitly]
public class MappingConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<UserEntity, UserResponseByLoginDto>
            .NewConfig()
            .Map(dest => dest.IsActive, src => src.RevokedOn == null);
    }
    
}