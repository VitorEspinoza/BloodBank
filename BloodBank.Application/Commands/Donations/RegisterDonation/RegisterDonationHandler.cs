using BloodBank.Application.ViewModels;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.Persistence;
using MediatR;

namespace BloodBank.Application.Commands.Donations.RegisterDonation;

public class RegisterDonationHandler : IRequestHandler<RegisterDonationCommand, ResultViewModel<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBloodDonorsRepository _bloodDonorsRepository;
    private readonly IDonationRepository _donationRepository;
    private readonly IOutboxRepository _outboxRepository;
    
    public RegisterDonationHandler(IUnitOfWork unitOfWork, IBloodDonorsRepository bloodDonorsRepository, IDonationRepository donationRepository, IOutboxRepository outboxRepository)
    {
        _unitOfWork = unitOfWork;
        _bloodDonorsRepository = bloodDonorsRepository;
        _donationRepository = donationRepository;
        _outboxRepository = outboxRepository;
    }

    public async Task<ResultViewModel<int>> Handle(RegisterDonationCommand request, CancellationToken cancellationToken)
    {
           return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var bloodDonor = await _bloodDonorsRepository
                    .GetById(request.BloodDonorId);
             
                var donation = request.ToEntity(bloodDonor!);
            
                await _donationRepository.AddAsync(donation);
            
                var @event = donation.RegisterDonationEvent(bloodDonor!.BloodType, bloodDonor.Email, bloodDonor.FullName);
            
                await _outboxRepository.SaveEvent(@event);
                return ResultViewModel<int>.Success(donation.Id);
            }, cancellationToken);
    }
}