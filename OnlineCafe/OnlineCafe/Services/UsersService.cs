using OnlineCafe.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using OnlineCafe.Storage;

namespace OnlineCafe.Services
{
    public interface IUsersService
    {
        Task Register(RegisterViewModel model);
        Task Login(LoginViewModel model);
        Task Logout();

        Task AddAddress(Guid userId, AddressViewModel model);
        Task<AccountProfileViewModel> GetInfo(Guid userId);
        Task<AddressViewModel> GetAddress(Guid userId, int AddressId);
        Task EditAddress(Guid userId, int AddressId, AddressViewModel model);
        Task DeleteAddress(Guid userId, int AddressId);
        Task EditProfile(Guid userId, int AccountId, EditAccountProfileViewModel model);

        Task<EditAccountProfileViewModel> GetInfoForEdit(Guid userId, AccountProfileViewModel model);
    }


    public class UsersService : IUsersService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UsersService(ApplicationDbContext context, UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task Register(RegisterViewModel model)
        {
            var user = new User
            {
                Email = model.Email,
                UserName = model.Email,
                BirthDate = model.BirthDate,
                Phone = model.Phone,
                Name = model.Name
            };
            var result =
                await _userManager.CreateAsync(user,
                    model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return;
            }

            var errors = string.Join(", ", result.Errors.Select(x => x.Description));
            throw new ArgumentException(errors);
        }

        public async Task Login(LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with email = {model.Email} does not found");
            }

            var claims = new List<Claim>
            {
                new("Name", user.Name),
                new("Id", user.Id.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            if (user.Roles?.Any() == true)
            {
                var roles = user.Roles.Select(x => x.Role).ToList();
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));
            }

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(2),
                IsPersistent = true
            };

            await _signInManager.SignInWithClaimsAsync(user, authProperties, claims);
        }

        public async Task Logout()
        {
            // Выход из системы == удаление куки
            await _signInManager.SignOutAsync();
        }


        public async Task<AccountProfileViewModel> GetInfo(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"User does not found");
            }

            List<Address> addresess = _context.UserAddress.Where(x => x.User == user).ToList();
            AccountProfileViewModel addresessDto = new AccountProfileViewModel();
            addresessDto.Addresses = new List<AddressViewModel>();
            foreach (Address address in addresess)
            {
                AddressViewModel addressDto = new AddressViewModel()
                {
                    Id = address.Id,
                    Flat = address.Flat,
                    House = address.House,
                    Entrance = address.Entrance,
                    Street = address.Street,
                    Note = address.Note,
                    Name = address.Name,
                    MainAddress = address.MainAddress,
                };
                addresessDto.Addresses.Add(addressDto);
            }

            addresessDto.Id = user.Id.ToString();
            addresessDto.Name = user.Name;
            addresessDto.Email = user.Email;
            addresessDto.Phone = user.Phone;
            addresessDto.BirthDate = user.BirthDate;
            return addresessDto;
        }


        public async Task AddAddress(Guid userId, AddressViewModel model)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"User does not found");
            }


            if (model.MainAddress == true)
            {
                Address mainAddress =
                    _context.UserAddress.FirstOrDefault(x => x.MainAddress == true && x.User.Id == user.Id);
                if (mainAddress is not null)
                {
                    throw new ArgumentException("Главных адресов не может быть более одного");
                }
            }
            else
            {
                Address mainAddress =
                    _context.UserAddress.FirstOrDefault(x => x.MainAddress == true && x.User.Id == user.Id);
                if (mainAddress is null)
                {
                    throw new ArgumentException("Первый адрес должен быть главным");
                }
            }

            Address address = new Address
            {
                User = user,
                Flat = model.Flat,
                House = model.House,
                Entrance = model.Entrance,
                Street = model.Street,
                Note = model.Note,
                Name = model.Name,
                MainAddress = model.MainAddress,
            };

            await _context.UserAddress.AddAsync(address);
            await _context.SaveChangesAsync();
        }


        public async Task<AddressViewModel> GetAddress(Guid userId, int AddressId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"User does not found");
            }

            Address address = _context.UserAddress.FirstOrDefault(x => x.User == user && x.Id == AddressId);
            if (address is null)
            {
                throw new KeyNotFoundException();
            }

            AddressViewModel model = new AddressViewModel()
            {
                Id = address.Id,
                Flat = address.Flat,
                House = address.House,
                Entrance = address.Entrance,
                Street = address.Street,
                Note = address.Note,
                Name = address.Name,
                MainAddress = address.MainAddress,
            };

            return model;
        }

        public async Task EditAddress(Guid userId, int AddressId, AddressViewModel model)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"User does not found");
            }

            Address address = _context.UserAddress.FirstOrDefault(x => x.User == user && x.Id == AddressId);


            if (model.MainAddress == true)
            {
                Address mainaddress = _context.UserAddress.FirstOrDefault(x =>
                    x.MainAddress == true && x.Id != address.Id && x.User.Id == user.Id);
                if (mainaddress is not null)
                {
                    throw new KeyNotFoundException("Главных адресов не может быть более одного");
                }
            }

            address.Name = model.Name;
            address.Street = model.Street;
            address.Note = model.Note;
            address.Flat = model.Flat;
            address.House = model.House;
            address.Entrance = model.Entrance;
            address.MainAddress = model.MainAddress;

            await _context.SaveChangesAsync();
        }


        public async Task DeleteAddress(Guid userId, int AddressId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"User does not found");
            }

            Address address = _context.UserAddress.FirstOrDefault(x => x.User == user && x.Id == AddressId);
            _context.UserAddress.Remove(address);


            await _context.SaveChangesAsync();
        }


        public async Task EditProfile(Guid userId, int AccountId, EditAccountProfileViewModel model)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"User does not found");
            }

            user.Name = model.Name;
            user.Phone = model.Phone;
            user.BirthDate = model.BirthDate;

            await _context.SaveChangesAsync();
        }


        public async Task<EditAccountProfileViewModel> GetInfoForEdit(Guid userId, AccountProfileViewModel model)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"User does not found");
            }


            EditAccountProfileViewModel Accountmodel = new EditAccountProfileViewModel()
            {
                Name = user.Name,
                Phone = user.Phone,
                BirthDate = user.BirthDate
            };

            return Accountmodel;
        }
    }
}