using AutoMapper;
using Stripe;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Core.Entities.Payments;
using PaymentMethod = UcarMobileApi.Core.Entities.Payments.PaymentMethod;


namespace UcarMobileApi.Application.Mapping;

public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<PaymentIntent, Payment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentMethodId, opt => opt.Ignore())
            .ForMember(d => d.ProviderPaymentId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.AmountCents, o => o.MapFrom(s => s.Amount))
            .ForMember(d => d.MetadataJson, o => o.MapFrom(s => s.Metadata))
            .ForMember(d => d.ErrorCode, o => o.MapFrom(s => s.LastPaymentError.Error));

        CreateMap<Payment, PaymentDto>().ReverseMap();

        CreateMap<PaymentMethodCard, PaymentMethod>();

        CreateMap<PaymentMethod, PaymentMethodDto>().ReverseMap();

        CreateMap<PaymentMethodCreateDto, PaymentMethod>();

        CreateMap<Refund, PaymentRefund>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.ProviderRefundId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.AmountCents, o => o.MapFrom(s => s.Amount));

        CreateMap<PaymentRefund, PaymentRefundResultDto>();


    }
}
