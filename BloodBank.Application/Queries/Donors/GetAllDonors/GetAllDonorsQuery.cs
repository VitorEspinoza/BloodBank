using BloodBank.Application.ViewModels;
using MediatR;

namespace BloodBank.Application.Queries.Donors.GetAllDonors;

public class GetAllDonorsQuery : IRequest<ResultViewModel<List<BloodDonorViewModel>>>;
