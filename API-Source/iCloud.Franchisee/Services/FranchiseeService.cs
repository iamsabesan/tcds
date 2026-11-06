using CommonServices;
using Consul.Filtering;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Email;
using EntitiesDTO.Material;
using EntitiesDTO.Payment;
using EntitiesDTO.Student;
using EntitiesDTO.Users;
using EntitiesDTO.Utilities;
using Frameworks.Entities;
using Frameworks.Helper;
using iCloud.Franchisee.Repository;
using MassTransit.Configuration;
using Newtonsoft.Json;
using SixLabors.ImageSharp.Formats.Png;
using System.Data;
using System.Globalization;
using Utilities.Extensions;

namespace iCloud.Franchisee.Services
{
    public class FranchiseeService
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IdentityInfo _userIdentityInfo;
        private string _connectionString;
        private readonly FranchiseeRepository _franchiseeRepository;
        private readonly CommonService _commonService;
        private readonly IWebHostEnvironment _environment;
        private readonly IDawnHttpClient _dawnHttpClient;

        #endregion
        #region Constructor
        public FranchiseeService(SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient, IWebHostEnvironment environment)
        {
            _sqlI18N = sqlI18N;
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _connectionString = accessor.GetConnectionString(dawnHttpClient);
            //_connectionString = "Data Source=3.108.251.251;Initial Catalog=iCloud_Franchisee_98392E8C_A6FC_48D6_873D_5A6C7C514B94_TCDS ;Persist Security Info=True;User ID=Identity_db ;Password=Ljgl5871$;TrustServerCertificate=Yes;";
            _commonService = new CommonService(dawnHttpClient);
            _franchiseeRepository = new FranchiseeRepository(_sqlI18N, _connectionString);
            _environment = environment;
            _dawnHttpClient = dawnHttpClient;
        }
        #endregion
        #region Methods

        ///// <summary>
        ///// Register Franchisee
        ///// </summary>
        ///// <param name="register"></param>
        ///// <param createdBy="Sakthivel"></param>
        ///// <param createdDate="03-04-2023"></param>
        ///// <returns></returns>
        ///// <exception cref="Exception"></exception>
        ///// <exception cref="DawnUserException"></exception>
        //public async Task<Guid> CreateFranchisee(FranchiseeRegister register)
        //{
        //    /* Create the tenant branch */
        //    BranchCreate branchCreate = register.Mapper<BranchCreate>();
        //    var branchID = await _commonService.CreateBranch(branchCreate);

        //    /* Create the contact */
        //    if (branchID == null)
        //        throw new Exception("Tenant branch id not found !");
        //    RandomPassword randomPassword = new RandomPassword();
        //    ContactModel contactCreate = register.Mapper<ContactModel>();
        //    contactCreate.Password = randomPassword.Next();
        //    contactCreate.TenantID = _userIdentityInfo.TenantID;
        //    contactCreate.TenantBranchID = Guid.Parse(branchID);
        //    contactCreate.AuthRoleID = new int[] { 2 };
        //    contactCreate.ContactType = "TenantUser";
        //    var userID = await _commonService.CreateUser(contactCreate);

        //    if (userID == null)
        //    {
        //        var rollBack = await _commonService.RollBackUser(userID, branchID);
        //    }

        //    /* Get contactID */
        //    var contactID = await _commonService.GetContactID(Guid.Parse(userID));

        //    /*Create contact address */
        //    if (contactID == null)
        //        throw new DawnUserException("Contact id not found !");
        //    ContactAddress contactAddress = register.Mapper<ContactAddress>();
        //    contactAddress.ContactID = Guid.Parse(contactID);
        //    contactAddress.AddressType = "Communication Address";
        //    contactAddress.ContactAddressID = Guid.NewGuid();
        //    var addressID = await _commonService.CreateContactAddress(contactAddress);

        //    /* RollBack user and contact */
        //    if (addressID == null)
        //    {
        //        var rollBackContact = await _commonService.RollBackUserAndBranch(contactID, userID, branchID);
        //        throw new Exception("address id not found !");
        //    }

        //    /* Create support documnet list */
        //    List<Images> franchiseeImages = new List<Images>();
        //    franchiseeImages.Add(new Images { DocumentType = "OwnerImage", Content = register.OwnerImage });
        //    franchiseeImages.Add(new Images { DocumentType = "OutletImage", Content = register.OutletImage });
        //    franchiseeImages.Add(new Images { DocumentType = "LaboratoryImage", Content = register.LaboratoryImage });
        //    franchiseeImages.Add(new Images { DocumentType = "ClassRoomImage", Content = register.ClassRoomImage });
        //    var supportDouments = await FranchiseeImageToDataTable(franchiseeImages);

        //    /* Create support documnet list */
        //    //FranchiseeRegisterDTO franchiseeRegister = new FranchiseeRegisterDTO();
        //    //franchiseeRegister.BranchID = Guid.Parse(branchID);
        //    //franchiseeRegister.ContactID = Guid.Parse(contactID);
        //    //franchiseeRegister.CreatedBy = Guid.Parse(contactID);
        //    //Guid result = _franchiseeRepository.RegisterFranchisee(franchiseeRegister, supportDouments);

        //    //if (result == Guid.Empty)
        //    //{
        //    //    var rollBackContactAddress = await _commonService.RollBackContactAddress(contactID, userID, branchID, addressID);
        //    //    throw new Exception("Could not register franchisee, cause by some invalid details !");
        //    //}
        //    return Guid.Empty;
        //}

        /// <summary>
        /// Rigister franchisee
        /// </summary>
        /// <param name="register"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Guid> RegisterFranchisee(FranchiseeRegister register)
        {
            if (register == null)
                throw new DawnUserException("Invalid data. ");
            List<Images> franchiseeImages = GetImages(register);
            var supportDouments = await FranchiseeImageToDataTable(franchiseeImages);
            DataTable banks = register.BankAccount.ToDataTable();
            if (_franchiseeRepository.CheckFranchiseeIsRejected(register.Email))
            {
                Guid franchiseeRequestID = _franchiseeRepository.FranchiseeRerequest(Guid.NewGuid(), register.Email, supportDouments);
                return franchiseeRequestID;
            }
            else
            {
                //Check email is verified
                await CheckRequiredForOTPValidation(register.EmailOTPTransID);
                if (_franchiseeRepository.CheckEmailIsExists(register.Email))
                    throw new DawnUserException("Email already exists !");
                EmailCheck emailCheck = new EmailCheck
                {
                    Email = register.Email
                };

                //Added on 16-04-25 for user login duplicate check Start
                dynamic emailAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.UserService.IsEmailCheck, emailCheck);
                bool res = emailAPIResult.Data;//.ToString();
                if (res == true)
                    throw new DawnUserException("Email already exists !");
                //Added on 16-04-25 for user login duplicate check end

                if (_franchiseeRepository.CheckPhoneIsExists(register.Phone))
                    throw new DawnUserException("Phone already exists !");
                foreach (var bank in register.BankAccount)
                {
                    if (_franchiseeRepository.CheckAccountNumberExists(bank.AccountNumber))
                        throw new DawnUserException($"The given account {bank.AccountNumber} is already exists");
                }
                FranchiseeRegisterDTO franchisee = register.Mapper<FranchiseeRegisterDTO>();
                Guid franchiseeRequestID = _franchiseeRepository.RegisterFranchisee(franchisee, supportDouments, Guid.NewGuid(), banks);
                return franchiseeRequestID;
            }
        }

        public List<Images> GetImages(FranchiseeRegister register)
        {
            var franchiseeImages = new List<Images> {
                new Images { DocumentType = "OwnerImage", Content = register.OwnerImage }
            };
            franchiseeImages.AddRange(register.OutletImage.Select(image => new Images { DocumentType = "OutletImage", Content = image }));
            franchiseeImages.AddRange(register.ClassRoomImage.Select(image => new Images { DocumentType = "ClassRoomImage", Content = image }));
            franchiseeImages.AddRange(register.LaboratoryImage.Select(image => new Images { DocumentType = "LaboratoryImage", Content = image }));
            return franchiseeImages;
        }


        /// <summary>
        /// Reject Franchisee
        /// </summary>
        /// <param name="franchisee"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string RejectFranchisee(FranchiseeRejection franchisee)
        {
            if (franchisee == null)
                throw new DawnUserException("Invalid data.");
            if (!_franchiseeRepository.CheckFranchiseeRequestID(franchisee.FranchiseeRequestID))
            {
                throw new DawnUserException("Invalid branch requestID !");
            }
            int result = _franchiseeRepository.RejectFranchisee(franchisee, _userIdentityInfo.ContactID);
            return result > 0 ? "Franchisee rejected" : throw new DawnUserException("Couldn't rejecet franchisee");
        }

        /// <summary>
        /// Activate franchisee
        /// </summary>
        /// <param name="franchisee"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Guid> ActivateFranchisee(ActivateFranchisee activate)
        {
            string branchID = string.Empty;
            string contactID = string.Empty;
            string addressID = string.Empty;
            string userID = string.Empty;
            string adminContactID = string.Empty;
            string bankRes = string.Empty;
            string paymentID = string.Empty;
            Guid result = Guid.Empty;
            Guid keyValue = Guid.Empty;
            try
            {
                if (!_franchiseeRepository.CheckFranchiseeRequestID(activate.FranchiseeRequestID))
                    throw new DawnUserException("Invalid branch request ID");
                var franchisee = _franchiseeRepository.GetFranchiseeDetail(activate.FranchiseeRequestID);
                BranchCreate branchCreate = franchisee.Mapper<BranchCreate>();
                branchCreate.TenantBranchName = franchisee.FranchiseeName;
                //Create TenantBranch
                branchID = await _commonService.CreateBranch(branchCreate);
                RandomPassword randomPassword = new RandomPassword();
                TenantContactUserWrite createUser = franchisee.Mapper<TenantContactUserWrite>();
                createUser.TenantBranchID = Guid.Parse(branchID);
                createUser.Password = randomPassword.Next();
                //createUser.Password = "India_123";
                //Create Franchisee
                contactID = await _commonService.CreateFranchisee(createUser);

                //Get UserID by ContactID
                userID = await _commonService.GetUserID(Guid.Parse(contactID));
                /*Create contact address */
                ContactAddress contactAddress = franchisee.Mapper<ContactAddress>();
                contactAddress.ContactID = Guid.Parse(contactID);
                contactAddress.AddressType = "Communication Address";
                contactAddress.ContactAddressID = Guid.NewGuid();
                contactAddress.Latitude = franchisee.Latitude.ToDecimal();
                contactAddress.Longitude = franchisee.Longitude.ToDecimal();
                contactAddress.Pincode = franchisee.Pincode.ToInt();
                addressID = await _commonService.CreateContactAddress(contactAddress);

                //Get admin contactID
                adminContactID = await _commonService.GetAdminContactID();
                //Create bank ledger for franchisee
                List<GetBanks> banks = _franchiseeRepository.GetBanks(activate.FranchiseeRequestID);
                bankRes = await BankCreation(banks, Guid.Parse(contactID), Guid.Parse(branchID));

                //Get franchisee payment details
                List<FranchiseePayments> franchiseePayments = _franchiseeRepository.GetFranchiseePayments(activate.FranchiseeRequestID);
                FranchiseePayments payments = new FranchiseePayments();
                payments = franchiseePayments.Where(x => x.FranchiseeRequestID == activate.FranchiseeRequestID).FirstOrDefault() ?? throw new Exception("");
                keyValue = payments.KeyValue;
                List<NewPaymentItems> newItems = new List<NewPaymentItems>();
                foreach (var paymentItems in franchiseePayments)
                {
                    FranchiseeToAdminPay paymentDetails = paymentItems.Mapper<FranchiseeToAdminPay>();
                    var paymentDetail = JsonConvert.SerializeObject(paymentDetails);
                    NewPaymentItems newPaymentItems = paymentItems.Mapper<NewPaymentItems>();
                    newPaymentItems.PaymentModeID = paymentItems.PaymodeID;
                    newPaymentItems.PayModeType = paymentItems.PaymodeType;
                    newPaymentItems.TransactionDate = DateTime.Parse(paymentItems.TransactionDate);
                    newPaymentItems.PaymentDetails = paymentDetail;
                    newItems.Add(newPaymentItems);
                }
                Receipt receipt = new Receipt();
                receipt.TenantBranchID = Guid.Parse(branchID);
                receipt.KeyTable = payments.KeyTable;
                receipt.KeyColumn = payments.KeyColumn;
                receipt.KeyValue = payments.KeyValue;
                receipt.TransactionName = payments.TransactionName;
                receipt.PaymentItems = newItems;
                receipt.CreatedBy = _userIdentityInfo.ContactID;
                receipt.VoucherDate = payments.VoucherDate.ToString().ToDateOnly();
                paymentID = await _commonService.CreatePayment(receipt);

                //Get admin branchID
                string adminBranchID = await _commonService.GetAdminBranchID(_userIdentityInfo.TenantID);
                bool posting = await FranchiseeToAdmin(franchiseePayments, Guid.Parse(branchID), Guid.Parse(adminBranchID), Guid.Parse(contactID), Guid.Parse(adminContactID), Guid.Parse(paymentID));
                result = _franchiseeRepository.ActivateFranchisee(activate, _userIdentityInfo.ContactID, Guid.Parse(branchID), Guid.Parse(contactID));

                //Send welcome response to franchisee after activation
                string res = await SendSuccessResponse(franchisee.Email, createUser.Password, franchisee.ContactName);
                return result;
            }
            catch (Exception ex)
            {
                if (branchID == null)
                    throw new DawnUserException("Tenant branch id not found !");
                if (contactID == null)
                {
                    string rollBackBranch = await _commonService.RollBackBranch(branchID);
                    throw new DawnUserException("Contact id not found !");
                }
                /* RollBack user and contact */
                if (addressID == null || addressID == "")
                {
                    string rollBackContact = await _commonService.RollBackUserAndBranch(userID, branchID);
                    throw new DawnUserException("Address id not found !");
                }
                if (bankRes == null)
                {
                    string rollBackContactAddress = await _commonService.RollBackContactAddress(contactID, userID, branchID, addressID);
                    throw new DawnUserException("Failed to create franchisee.");
                }
                if (paymentID == null || paymentID == string.Empty)
                {
                    string rollBackContactAddress = await _commonService.RollBackContactAddress(contactID, userID, branchID, addressID);
                    throw new DawnUserException("Payment id not found !");
                }
                if (result == Guid.Empty)
                {
                    string rollbackPayment = await _commonService.RollBackPayment(keyValue);
                    var rollback = await _commonService.RollBackContactAddress(contactID, userID, branchID, addressID);
                    throw new DawnUserException("Couldn't activate franchisee");
                }
                throw new DawnUserException(ex.Message);
            }
        }

        private async Task<DataTable> FranchiseeImageToDataTable(List<Images> franchiseeImages)
        {
            List<SupportDocument> supportDocuments = new List<SupportDocument>();
            foreach (Images item in franchiseeImages)
            {
                using (var ms = new MemoryStream())
                {
                    await item.Content.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    supportDocuments.Add(new SupportDocument
                    {
                        DocumentType = item.DocumentType,
                        FileName = item.Content.FileName,
                        Content = fileBytes,
                        ContentType = item.Content.ContentType
                    });
                }
            }
            return supportDocuments.ToDataTable();
        }

        /// <summary>
        /// Get franchisee inactive list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public async Task<IEnumerable<FranchiseeInfo>> GetFranchiseeInfos()
        {
            List<FranchiseeInfo> franchiseeInfos = new List<FranchiseeInfo>();
            DataSet requestDataSet = _franchiseeRepository.GetFranchiseeRequest();
            var result = _franchiseeRepository.GetBranchDetails();
            foreach (var item in result)
            {
                FranchiseeInfo franchiseeInfo = new FranchiseeInfo();
                franchiseeInfo.FranchiseeEnquiryID = item.FranchiseeEnquiryID;
                franchiseeInfo.FranchiseeRequestID = item.FranchiseeRequestID;
                franchiseeInfo.FranchiseeCode = item.FranchiseeCode;
                franchiseeInfo.FranchiseeName = item.FranchiseeName;
                franchiseeInfo.ContactName = item.ContactName;
                franchiseeInfo.Gender = item.Gender;
                franchiseeInfo.Phone = item.Phone;
                franchiseeInfo.AlternatePhone = item.AlternatePhone;
                franchiseeInfo.Email = item.Email;
                franchiseeInfo.Status = item.Status;
                franchiseeInfo.Reason = item.Reason;
                franchiseeInfo.RequestDate = item.RequestDate;
                franchiseeInfo.StatusDate = item.StatusDate;
                franchiseeInfo.Address = requestDataSet.Tables[3].DataTablesToList<FranchiseeAddress>().Where(x => x.FranchiseeRequestID == item.FranchiseeRequestID).ToList();
                if (item.Status == "PENDING")
                {
                    franchiseeInfo.FeesDetails = requestDataSet.Tables[1].DataTablesToList<FeesDetail>();
                }
                else if (item.Status == "APPROVED")
                {
                    franchiseeInfo.FeesDetails = requestDataSet.Tables[0].DataTablesToList<FeesDetail>().Where(x => x.FranchiseeEnquiryID == item.FranchiseeEnquiryID).ToList();
                }
                else if (item.Status == "PAID REQUEST" || item.Status == "REJECTED(PAYMENT)")
                {
                    Guid packageLogID = _franchiseeRepository.GetPackageLogID(item.FranchiseeEnquiryID);
                    //franchiseeInfo.PaymentDetails = _franchiseeRepository.GetFranchiseePaymentHistory(packageLogID);
                    franchiseeInfo.PaymentDetails = await GetFranchiseePaymentHistory(packageLogID, item.FranchiseeRequestID);
                    franchiseeInfo.PackageLogID = packageLogID;
                }
                franchiseeInfo.FranchiseeImgs = requestDataSet.Tables[2].DataTablesToList<FranchiseeImg>().Where(x => x.FranchiseeRequestID == item.FranchiseeRequestID).ToList();
                franchiseeInfos.Add(franchiseeInfo);
            }
            return franchiseeInfos;
        }

        public async Task<List<Payments>> GetFranchiseePaymentHistory(Guid packageLogID, Guid franchiseeRequestID)
        {
            List<Payments> payments = new List<Payments>();
            var franchiseePayment = _franchiseeRepository.GetFranchiseePaymentHistory(packageLogID);
            foreach (var item in franchiseePayment)
            {
                Payments payment = new Payments();
                payment.Amount = item.Amount;
                payment.TransactionDate = item.TransactionDate;
                payment.Summary = item.Summary;
                payment.IsCancelled = item.IsCancelled;
                payment.Reason = item.Reason;
                payment.PayMode = item.PayMode;
                var document = await _commonService.GetDocumentID(item.FranchiseePaymentTransID, item.PayMode);
                payment.DocumentID = Guid.Parse(document);
                payments.Add(payment);
            }
            return payments;
        }


        /// <summary>
        /// Get franchisee request status
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public async Task<FranchiseeRequestInfo> GetFranchiseeRequestInfo(Guid franchiseeRequestID)
        {
            DataSet requestDataSet = _franchiseeRepository.GetFranchiseeRequestInfo(franchiseeRequestID);
            var franchisee = requestDataSet.Tables[0].DataTablesToItem<BranchDetail2>();
            //var payments = _franchiseeRepository.GetFranchiseePaymentHistory(franchisee.PackageLogID);
            FranchiseeRequestInfo franchiseeInfo = franchisee.Mapper<FranchiseeRequestInfo>();
            franchiseeInfo.BranchName = franchisee.FranchiseeName;
            franchiseeInfo.Address = requestDataSet.Tables[3].DataTablesToList<FranchiseeAddress>().Where(x => x.FranchiseeRequestID == franchiseeRequestID).ToList();
            if (franchiseeInfo.Status == "APPROVED" || franchiseeInfo.Status == "REJECTED(PAYMENT)")
            {
                franchiseeInfo.FeesDetails = requestDataSet.Tables[1].DataTablesToList<FeesDetail>();
            }
            franchiseeInfo.FranchiseeImgs = requestDataSet.Tables[2].DataTablesToList<FranchiseeImg>().Where(x => x.FranchiseeRequestID == franchisee.FranchiseeRequestID).ToList();
            franchiseeInfo.Payments = await PaymentHistory(franchisee.PackageLogID);
            franchiseeInfo.Banks = _franchiseeRepository.GetFranchiseeBanks(franchiseeRequestID);
            return franchiseeInfo;
        }

        /// <summary>
        /// Get franchisee profile info
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<FranchiseeProfile> GetFranchiseeProfileInfo()
        {
            DataSet requestDataSet = _franchiseeRepository.GetFranchiseeProfileInfo(_userIdentityInfo.TenantBranchID);
            var franchisee = requestDataSet.Tables[0].DataTablesToList<BranchDetail1>();
            //var franchiseeImage = requestDataSet.Tables[1].DataTablesToList<FranchiseeImg>();
            List<FranchiseeImg> franchiseeImage = new List<FranchiseeImg>();
            foreach (DataRow row in requestDataSet.Tables[1].Rows)
            {
                byte[]? attachment = row["AttachmentContent"] == DBNull.Value ? null : (byte[])row["AttachmentContent"];
                FranchiseeImg img = new FranchiseeImg
                {
                    FranchiseeRequestID = Guid.TryParse(row["FranchiseeRequestID"]?.ToString(), out var guid) ? guid : Guid.Empty,
                    DocumentName = row["DocumentName"]?.ToString() ?? string.Empty,
                    FileName = row["FileName"]?.ToString() ?? string.Empty,
                    DocumentType = row["DocumentType"]?.ToString() ?? string.Empty,
                    ContentUrl = row["ContentUrl"]?.ToString() ?? string.Empty,
                    AttachmentContent = attachment,
                    ThumbnailData = attachment != null ? MakeThumbnail(attachment, 64, 64) : string.Empty
                };
                franchiseeImage.Add(img);
            }

            DataSet tenantDataSet = await _commonService.GetTenantBranchAndConatctByID(_userIdentityInfo.TenantBranchID);
            var branchList = tenantDataSet.Tables[0].DataTablesToList<FranchiseeBaseInfo>();
            var addressList = tenantDataSet.Tables[1].DataTablesToList<AddressInfo>();
            decimal walletBalance = await _commonService.GetWalletBalance(_userIdentityInfo.TenantBranchID);
            //Added on 19-05-2025
            foreach (var f in franchisee)
            {
                var matchingAddresses = addressList.Where(a => branchList.Any(b => b.TenantBranchID == f.TenantBranchID && b.ContactID == a.ContactID));
                foreach (var addr in matchingAddresses)
                {
                    addr.Taluk = f.Taluk;
                }
            }
            //Added on 19-05-2025
            var franchiseeInfo = (from f in franchisee
                                  join fi in franchiseeImage on f.FranchiseeRequestID equals fi.FranchiseeRequestID
                                  join b in branchList on f.TenantBranchID equals b.TenantBranchID
                                  //join a in addressList on b.ContactID equals a.ContactID
                                  select new FranchiseeProfile()
                                  {
                                      FranchiseeRequestID = f.FranchiseeRequestID,
                                      FranchiseeCode = f.FranchiseeCode,
                                      BranchName = b.TenantBranchName,
                                      ContactID = b.ContactID,
                                      ContactName = b.ContactName,
                                      Gender = b.Gender,
                                      Phone = b.Phone,
                                      AlternatePhone = b.AlternatePhone,
                                      Email = b.Email,
                                      WalletBalance = walletBalance,
                                      ApprovalStartDate = f.ApprovalStartDate,
                                      ApprovalEndDate = f.ApprovalEndDate,
                                      SubscriptionStartDate = f.SubscriptionStartDate,
                                      SubscriptionEndDate = f.SubscriptionEndDate,
                                      Address = addressList.Where(x => x.ContactID == b.ContactID).ToList(),
                                      FranchiseeImgs = franchiseeImage
                                  }).FirstOrDefault();

            return franchiseeInfo;
        }

        /// <summary>
        /// Get Active franchisee list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public async Task<IEnumerable<FranchiseeActiveList>> GetActiveFranchiseeList()
        {
            List<FranchiseeActiveList> franchiseeActiveLists = new List<FranchiseeActiveList>();
            var franchiseeList = _franchiseeRepository.GetActiveFranchiseeList();            
            //var branchContact = await _commonService.ActiveFranchiseeContact(Guid.Empty);
            var branchContact =  _franchiseeRepository.GetActiveFranchiseeListV1();
            string date = DateTime.Now.ToString();
            foreach (var item in franchiseeList)
            {
                var branch = branchContact.Where(x => x.TenantBranchID == item.TenantBranchID).FirstOrDefault();
                if (branch != null)
                {
                    FranchiseeActiveList franchiseeActiveList = new FranchiseeActiveList();
                    franchiseeActiveList.TenantBranchID = item.TenantBranchID;
                    franchiseeActiveList.FranchiseeCode = item.FranchiseeCode;
                    franchiseeActiveList.BranchName = branch.BranchName;
                    franchiseeActiveList.ContactName = branch.ContactName;
                    franchiseeActiveList.Phone = branch.Phone;
                    franchiseeActiveList.AlternatePhone = branch.AlternatePhone;
                    franchiseeActiveList.Email = branch.Email;
                    franchiseeActiveList.Area = branch.Area;
                    franchiseeActiveList.District = branch.District;
                    franchiseeActiveList.State = branch.State;
                    franchiseeActiveList.Taluk = branch.Taluk;

                    DateTime approvalEndDate = GetDateTime(item.ApprovalEndDate);
                    DateTime subscriptionEndDate = GetDateTime(item.SubscriptionEndDate);
                    DateTime registrationDate = GetDateTime(item.RegistrationDate);

                    //DateTime approvalEndDate = DateTime.Parse(item.ApprovalEndDate);
                    //DateTime subscriptionEndDate = DateTime.Parse(item.SubscriptionEndDate);
                    //DateTime registrationDate = DateTime.Parse(item.RegistrationDate);
                    //franchiseeActiveList.ApprovalEndDate = DateTime.Parse(item.ApprovalEndDate).ToString(_userIdentityInfo.DateTimeFormat);
                    //franchiseeActiveList.SubscriptionEndDate = DateTime.Parse(item.SubscriptionEndDate).ToString(_userIdentityInfo.DateTimeFormat);
                    //franchiseeActiveList.RegistrationDate = DateTime.Parse(item.RegistrationDate).ToString(_userIdentityInfo.DateTimeFormat);
                    //string dateconv = string.Empty;
                    //franchiseeActiveList.ApprovalEndDate = (dateconv.ToDateTime()).ToString(_userIdentityInfo.DateTimeFormat);
                    //franchiseeActiveList.ApprovalEndDate = DateTime.Parse(dateconv).ToString(_userIdentityInfo.DateTimeFormat);
                    //franchiseeActiveList.ApprovalEndDate = item.ApprovalEndDate.ToString(_userIdentityInfo.DateTimeFormat);

                    franchiseeActiveList.ApprovalEndDate = approvalEndDate.ToString(_userIdentityInfo.DateTimeFormat);
                    franchiseeActiveList.SubscriptionEndDate = subscriptionEndDate.ToString(_userIdentityInfo.DateTimeFormat);
                    franchiseeActiveList.RegistrationDate = registrationDate.ToString(_userIdentityInfo.DateTimeFormat);

                    franchiseeActiveList.Status = item.IsActive;

                    // Calculate day differences
                    var utcNow = DateTime.Now;
                    var approvalCount = (approvalEndDate.Date - utcNow.Date).TotalDays;
                    var subscriptionCount = (subscriptionEndDate.Date - utcNow.Date).TotalDays;

                    //double approvalCount = item.ApprovalEndDate.Date.Subtract(DateTime.Now.Date).TotalDays;
                    //double subscriptionCount = item.SubscriptionEndDate.Date.Subtract(DateTime.Now.Date).TotalDays;

                    //if (item.ApprovalEndDate < DateTime.Now || item.SubscriptionEndDate < DateTime.Now)
                    if (approvalEndDate < utcNow || subscriptionEndDate < utcNow)
                    {
                        franchiseeActiveList.SubscriptionStatus = "Expired";
                    }
                    else if (approvalCount >= 15 || subscriptionCount >= 15)
                    {
                        franchiseeActiveList.SubscriptionStatus = "Live";
                    }
                    else if (approvalCount < 15 || subscriptionCount < 15)
                    {
                        franchiseeActiveList.SubscriptionStatus = "Alert";
                    }
                    franchiseeActiveLists.Add(franchiseeActiveList);
                }
            }
            return franchiseeActiveLists;
        }
        /*To checking purpose extension*/
        private static DateTime GetDateTime(object dateValue)
        {
            return dateValue switch
            {
                DateTime dt => dt,
                DateTimeOffset dto => dto.DateTime,
                string s => DateTimeOffset.Parse(s).DateTime,
                //string dts => DateTime.Parse(dts),
                _ => throw new ArgumentException("Unsupported date type")
            };
        }

        /// <summary>
        /// Create franchisee registration payment
        /// </summary>
        /// <param name="payment"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="13-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> CreateFranchiseePayment(EntitiesDTO.Admin.Payment payment)
        {
            SubscriptionLog subscription = new SubscriptionLog();
            subscription.FranchiseeRequestID = payment.FranchiseeRequestID;
            if (!_franchiseeRepository.CheckFranchiseeRequestID(subscription.FranchiseeRequestID))
                throw new DawnUserException("Invalid branch request ID !");
            SubscriptionLogInfo log;
            if (_franchiseeRepository.CheckPackageLogIDExists(subscription.FranchiseeRequestID) != Guid.Empty)
            {
                Guid packageLogID = _franchiseeRepository.CheckPackageLogIDExists(subscription.FranchiseeRequestID);
                log = _franchiseeRepository.GetPackageLogInfo(subscription.FranchiseeRequestID, packageLogID);
            }
            else
            {
                log = _franchiseeRepository.CreateSubscriptionLog(subscription, _userIdentityInfo.ContactID);
            }
            SubscriptionLogInfo info = log.Mapper<SubscriptionLogInfo>();
            FranchiseePayment franchisee = new FranchiseePayment();
            franchisee.FranchiseeRequestID = payment.FranchiseeRequestID;
            franchisee.KeyTable = "ApprovalAndSubscriptionLog";
            franchisee.KeyColumn = "PackageLogID";
            franchisee.KeyValue = info.PackageLogID;
            franchisee.TransactionName = info.PackageType + " FEES";
            franchisee.CreatedBy = _userIdentityInfo.ContactID;
            DataTable paymentItem = payment.PaymentItems.ToDataTable();
            Guid franchiseePaymentID = _franchiseeRepository.FranchiseePayment(franchisee, paymentItem);
            TempDocumentActivate temp = new TempDocumentActivate();
            List<DocumentItem> documents = new List<DocumentItem>();
            foreach (var item in payment.PaymentItems)
            {
                if (item.DocumentID != Guid.Empty)
                {
                    Guid transID = _franchiseeRepository.GetTransID(franchiseePaymentID, item.TransactionNo);
                    documents.Add(new DocumentItem { DocumentID = item.DocumentID, EntityID = transID });
                    temp.DocumentItem = documents;
                }
            }
            if (temp.DocumentItem != null)
            {
                var activateDocument = await _commonService.TempDocumentActivated(temp);
            }
            return franchiseePaymentID != Guid.Empty ? "Registration payment successfull !" : throw new DawnUserException("Payment Failed.");
        }

        /// <summary>
        /// Create franchisee renewal
        /// </summary>
        /// <param name="payment"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<Guid> FranchiseeApprovalRenewalPayment(RenewalPayment payment)
        {
            SubscriptionLogInfo logInfo = new SubscriptionLogInfo();
            RenewalSubscriptionLog subscription = new RenewalSubscriptionLog();
            subscription.TenantBranchID = _userIdentityInfo.TenantBranchID;
            subscription.PackageID = payment.PackageID;
            subscription.PaymentType = payment.PaymentType;
            if (payment.PaymentType.ToConditionFormat() != "WALLET")
            {
                logInfo = _franchiseeRepository.RenewalSubscriptionLog(subscription, _userIdentityInfo.ContactID);
                List<NewPaymentItems> newItems = new List<NewPaymentItems>();
                foreach (var paymentItems in payment.PaymentItems)
                {
                    FranchiseeToAdminPay paymentDetails = paymentItems.Mapper<FranchiseeToAdminPay>();
                    var paymentDetail = JsonConvert.SerializeObject(paymentDetails);
                    NewPaymentItems newPaymentItems = paymentItems.Mapper<NewPaymentItems>();
                    newPaymentItems.BankName = paymentItems.FromLedgerName;
                    newPaymentItems.TransactionDate = paymentItems.TransactionDate;
                    newPaymentItems.PaymentDetails = paymentDetail;
                    newItems.Add(newPaymentItems);
                }
                SubscriptionLogInfo info = logInfo.Mapper<SubscriptionLogInfo>();
                Receipt receipt = new Receipt();
                receipt.TenantBranchID = _userIdentityInfo.TenantBranchID;
                receipt.KeyTable = "ApprovalAndSubscriptionLog";
                receipt.KeyColumn = "PackageLogID";
                receipt.KeyValue = info.PackageLogID;
                string transactionName = info.PackageType + " RENEWAL";
                receipt.TransactionName = transactionName;
                receipt.PaymentItems = newItems;
                receipt.CreatedBy = _userIdentityInfo.ContactID;
                //receipt.VoucherDate = DateTime.Now.Date.ToString().ToDateOnly();
                receipt.VoucherDate = DateTime.Now.ToString("yyyy-MM-dd");
                string payments = await _commonService.CreatePayment(receipt);
                if (payments == null || payments == string.Empty)
                {
                    int result = _franchiseeRepository.RollBackSubscriptionLog(logInfo.PackageLogID);
                    throw new DawnUserException("Payment failed");
                }
                TempDocumentActivate temp = new TempDocumentActivate();
                List<DocumentItem> documents = new List<DocumentItem>();
                foreach (var item in payment.PaymentItems)
                {
                    if (item.DocumentID != Guid.Empty)
                    {
                        string transID = await _commonService.GetPaymentTransID(Guid.Parse(payments), item.TransactionNo);
                        documents.Add(new DocumentItem { DocumentID = item.DocumentID, EntityID = Guid.Parse(transID) });
                        temp.DocumentItem = documents;
                    }
                }
                if (temp.DocumentItem != null)
                {
                    var activateDocument = await _commonService.TempDocumentActivated(temp);
                }
                string adminContactID = await _commonService.GetAdminContactID();
                string adminBranchID = await _commonService.GetAdminBranchID(_userIdentityInfo.TenantID);
                bool posting = await _commonService.FranchiseeToAdmin(payment.PaymentItems, _userIdentityInfo.TenantBranchID, Guid.Parse(adminContactID), _userIdentityInfo.ContactID, Guid.Parse(adminBranchID), Guid.Parse(payments), transactionName);
                if (posting == false)
                {
                    var rollback = await _commonService.RollBackPayment(info.PackageLogID);
                    int result = _franchiseeRepository.RollBackSubscriptionLog(logInfo.PackageLogID);
                    throw new DawnUserException("Payment failed");
                }
                return Guid.Parse(payments);
            }
            else
            {
                Guid result = await WalletRenewalPayment(subscription, payment);
                return result;
            }
        }

        public async Task<Guid> WalletRenewalPayment(RenewalSubscriptionLog subscription, RenewalPayment payment)
        {
            decimal walletBalance = await _commonService.GetWalletBalance(_userIdentityInfo.TenantBranchID);
            //Check franchisee wallet balance
            if (payment.TransactionAmount > walletBalance)
                throw new DawnUserException("Insufficient wallet balance");
            SubscriptionLogInfo logInfo = new SubscriptionLogInfo();
            logInfo = _franchiseeRepository.WalletRenewalPayment(subscription, _userIdentityInfo.ContactID);
            foreach (var item in payment.PaymentItems)
            {
                string giverContactID = await _commonService.GetFranchiseeContactID(_userIdentityInfo.TenantBranchID);
                string receiverContactID = await _commonService.GetAdminContactID();
                Guid giverBranchID = _userIdentityInfo.TenantBranchID;
                string receiverBrabchID = await _commonService.GetAdminBranchID(_userIdentityInfo.TenantID);
                //string transactionName = logInfo.PackageType + " RENEWAL";
                string transactionName = $"{logInfo.PackageType} RENEWAL ( {logInfo.PackageStartDate} to {logInfo.PackageEndDate} )";
                NewPaymentDetails details = new NewPaymentDetails();
                var branchBanks = await _commonService.GetBranchBankList(_userIdentityInfo.TenantBranchID);
                var adminBanks = await _commonService.GetAdminBankList();
                var branchBank = branchBanks.Where(x => x.LedgerName.ToConditionFormat() == "WALLETBANK").FirstOrDefault();
                var adminBank = adminBanks.Where(x => x.LedgerName.ToConditionFormat() == "WALLETBANK").FirstOrDefault();
                details.PaymentModeID = item.PaymentModeID;
                details.PayModeType = item.PayModeType;
                details.PayModeValue = item.Amount;
                details.Amount = item.Amount;
                details.FromLedgerID = branchBank.LedgerID;
                details.FromLedgerName = branchBank.LedgerName;
                details.ToLedgerID = adminBank.LedgerID;
                details.ToLedgerName = adminBank.LedgerName;
                //details.TransactionDate = DateTime.Now.ToString();
                details.TransactionDate = item.TransactionDate.ToString();
                bool posting = await _commonService.WalletToWalletEntry(details, giverBranchID, Guid.Parse(receiverBrabchID), Guid.Parse(giverContactID), Guid.Parse(receiverContactID), logInfo.PackageLogID, transactionName);
                WalletRenewalPayment walletRenewal = new WalletRenewalPayment();
                walletRenewal.WalletType = logInfo.PackageType;
                walletRenewal.TenantBranchID = _userIdentityInfo.TenantBranchID;
                walletRenewal.SenderID = Guid.Parse(giverContactID);
                walletRenewal.ReceiverID = Guid.Parse(receiverContactID);
                walletRenewal.WalletOut = item.Amount;
                walletRenewal.Description = transactionName;
                walletRenewal.CreatedBy = _userIdentityInfo.ContactID;
                int renewalPayment = await _commonService.CreateRenewalPayment(walletRenewal);
            }
            return logInfo.PackageLogID;
        }

        /// <summary>
        /// Get franchisee profile by id
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public async Task<FranchiseeProfileInfo> GetFranchiseeProfileByID(Guid tenantBranchID, string fromDate, string toDate)
        {
            FranchiseeProfileInfo franchiseeInfo = new FranchiseeProfileInfo();
            DataSet requestDataSet = _franchiseeRepository.GetFranchiseeProfileInfo(tenantBranchID);
            var result = requestDataSet.Tables[0].DataTablesToList<BranchDetail1>();
            DataSet tenantDataSet = await _commonService.GetTenantBranchAndConatctByID(tenantBranchID);
            var branchList = tenantDataSet.Tables[0].DataTablesToList<FranchiseeBaseInfo>();
            //var addressList = tenantDataSet.Tables[1].DataTablesToList<AddressInfo>();
            var addressList = requestDataSet.Tables[2].DataTablesToList<AddressInfo>();
            List<Wallet> wallet = await _commonService.GetBranchWalletTrans(tenantBranchID);
            var totalPurchase = await _commonService.GetBranchTotalPurchase(tenantBranchID, fromDate, toDate);
            foreach (var item in result)
            {
                FranchiseeBaseInfo franchiseeBaseInfo = new FranchiseeBaseInfo();
                franchiseeBaseInfo = branchList.Where(x => x.TenantBranchID == item.TenantBranchID).FirstOrDefault()!;
                Purchase purchase = new Purchase();
                purchase.Books = totalPurchase.Books;
                purchase.ExamIndent = totalPurchase.Exam;
                purchase.Others = 0;
                if (franchiseeBaseInfo != null)
                {
                    franchiseeInfo.FranchiseeRequestID = item.FranchiseeRequestID;
                    franchiseeInfo.FranchiseeCode = item.FranchiseeCode;
                    franchiseeInfo.FranchiseeName = franchiseeBaseInfo.TenantBranchName != null ? franchiseeBaseInfo.TenantBranchName : "";
                    franchiseeInfo.ContactID = franchiseeBaseInfo.ContactID;
                    franchiseeInfo.ContactName = franchiseeBaseInfo.ContactName;
                    franchiseeInfo.Gender = franchiseeBaseInfo.Gender;
                    franchiseeInfo.Phone = franchiseeBaseInfo.Phone;
                    franchiseeInfo.AlternatePhone = franchiseeBaseInfo.AlternatePhone;
                    franchiseeInfo.Email = franchiseeBaseInfo.Email;
                    franchiseeInfo.ApprovalStartDate = item.ApprovalStartDate;
                    franchiseeInfo.ApprovalEndDate = item.ApprovalEndDate;
                    franchiseeInfo.SubscriptionStartDate = item.SubscriptionStartDate;
                    franchiseeInfo.SubscriptionEndDate = item.SubscriptionEndDate;
                    franchiseeInfo.Address = addressList.Where(x => x.ContactID == franchiseeBaseInfo.ContactID).ToList();
                    //franchiseeInfo.FranchiseeImgs = requestDataSet.Tables[1].DataTablesToList<FranchiseeImg>().Where(x => x.FranchiseeRequestID == item.FranchiseeRequestID).ToList();
                    franchiseeInfo.WalletTrans = wallet;
                    franchiseeInfo.RenewalTrans = _franchiseeRepository.GetFranchiseeRenewalList(item.FranchiseeEnquiryID);
                    franchiseeInfo.TotalPurchases = purchase;
                    foreach(DataRow row in requestDataSet.Tables[1].Rows)
                    {
                        byte[]? attachment =row["AttachmentContent"] == DBNull.Value ? null : (byte[])row["AttachmentContent"];
                        FranchiseeImg img = new FranchiseeImg
                        {
                            FranchiseeRequestID = Guid.TryParse(row["FranchiseeRequestID"]?.ToString(), out var guid) ? guid : Guid.Empty,
                            DocumentName = row["DocumentName"]?.ToString() ?? string.Empty,
                            FileName = row["FileName"]?.ToString() ?? string.Empty,
                            DocumentType = row["DocumentType"]?.ToString() ?? string.Empty,
                            ContentUrl = row["ContentUrl"]?.ToString() ?? string.Empty,
                            AttachmentContent = attachment,     
                            ThumbnailData =  attachment != null ? MakeThumbnail(attachment,64,64) : string.Empty 
                        };
                        franchiseeInfo.FranchiseeImgs.Add(img);
                    }
                }
            }
            return franchiseeInfo;
        }
        private string MakeThumbnail(byte[] myImage, int thumbWidth, int thumbHeight)
        {            
            using (var image = SixLabors.ImageSharp.Image.Load(myImage)) // Load the image from the byte array
            {
                // Resize the image to the specified dimensions while maintaining the aspect ratio
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new SixLabors.ImageSharp.Size(thumbWidth, thumbHeight),
                    Mode = ResizeMode.Max // Ensures the image is resized while keeping the original aspect ratio
                }));
                // Save the thumbnail to a MemoryStream
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, new PngEncoder()); // Save the image as PNG
                    return Convert.ToBase64String(ms.ToArray()); // Convert to Base64 string
                }
            }
        }
        /// <summary>
        /// Get franchisee basic info
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="29-04-2023"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GetFranchiseeBasicInfo>> GetFranchiseeBasicInfo()
        {
            List<GetFranchiseeBasicInfo> franchiseeInfos = new List<GetFranchiseeBasicInfo>();
            List<GetFranchiseeCode> franchisee = _franchiseeRepository.GetFranchiseeBasicInfo();
            DataTable branchIdsTable = new DataTable();
            branchIdsTable.Columns.Add("TenantBranchID", typeof(Guid));
            foreach (var item in franchisee)
            {
                branchIdsTable.Rows.Add(item.TenantBranchID);
            }
            string franchiseeConn = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepo = new CommonRepository(_sqlI18N, franchiseeConn);
            var branchDetailsList = commonRepo.GetBranchDetail(branchIdsTable);
            foreach (var franchisees in franchisee)
            {
                //var branches = await _commonService.GetBranchDetails(franchisees.TenantBranchID);
                var branches = branchDetailsList.Where(a => a.TenantBranchID == franchisees.TenantBranchID).FirstOrDefault();
                if (branches != null )
                {
                    GetFranchiseeBasicInfo franchiseeBasicInfo = branches.Mapper<GetFranchiseeBasicInfo>();
                    franchiseeBasicInfo.TenantBranchID = franchisees.TenantBranchID;
                    franchiseeBasicInfo.FranchiseeCode = franchisees.FranchiseeCode;
                    franchiseeInfos.Add(franchiseeBasicInfo);
                }
            }
            return franchiseeInfos;
        }

        /// <summary>
        /// Get franchisee code
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="02-05-2023"></param>
        /// <returns>
        /// Returns franchisee codes
        /// </returns>
        public async Task<IEnumerable<GetFranchiseeCode>> GetFranchiseeID()
        {
            return _franchiseeRepository.GetFranchiseeID();
        }

        /// <summary>
        /// Reject franchisee renewal request
        /// </summary>
        /// <param name="packageLogID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<string> RejectRenewalRequest(RejectRenewal reject)
        {
            GetPaymentID paymentID = await _commonService.GetPaymentID(reject.PackageLogID);
            List<FranchiseeToAdminPay> payments = await _commonService.GetPaymentItems(reject.PackageLogID);
            Guid giverBranchID = _userIdentityInfo.TenantBranchID;
            string receiverContactID = await _commonService.GetFranchiseeContactID(paymentID.TenantBranchID);
            string giverContactID = await _commonService.GetAdminContactID();
            string packageType = _franchiseeRepository.GetPackageType(reject.PackageLogID);
            string transactionName = $"{packageType} REJECTED";
            RejectPayment payment = new RejectPayment();
            payment.PackageLogID = reject.PackageLogID;
            payment.Reason = reject.Reason;
            var rejectPayment = await _commonService.RejectPaidRequest(payment);
            bool posting = await _commonService.FranchiseeToAdminReEntry(payments, giverBranchID, paymentID.TenantBranchID, Guid.Parse(giverContactID), Guid.Parse(receiverContactID), paymentID.TenantBranchTransID, transactionName);
            int result = _franchiseeRepository.RejectRenewalRequest(reject.PackageLogID, reject.Reason, _userIdentityInfo.ContactID);
            return result > 0 ? "Request rejected" : throw new DawnUserException("Couldn't reject request");
        }

        /// <summary>
        /// Get franchisee renewal request list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<List<RenewalRequestList>> getRenewalRequestList()
        {
            List<RenewalRequestList> renewalRequestLists = new List<RenewalRequestList>();
            var renewalList = _franchiseeRepository.GetFranchiseeRenewalList();
            var franchiseeIDs = await _commonService.GetFranchiseeID();
            DataTable branchIdsTable = new DataTable();
            branchIdsTable.Columns.Add("TenantBranchID", typeof(Guid));
            foreach (var item in franchiseeIDs)
            {
                branchIdsTable.Rows.Add(item.TenantBranchID);
            }
            //string franchiseeConn = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepo = new CommonRepository(_sqlI18N, _connectionString);
            var branchTalukList = commonRepo.GetFranchiseeCode(branchIdsTable);

            foreach (var renewal in renewalList)
            {
                var branch = await _commonService.GetBranchDetails(renewal.TenantBranchID);
                var franchiseeID = franchiseeIDs.Where(x => x.TenantBranchID == branch.TenantBranchID).FirstOrDefault();
                //var paymentDetails = await _commonService.GetPaymentDetails(renewal.PackageLogID);
                RenewalRequestList requestList = new RenewalRequestList();
                requestList.RequestDate = renewal.RequestDate;
                requestList.TenantBranchID = renewal.TenantBranchID;
                requestList.PackageLogID = renewal.PackageLogID;
                requestList.FranchiseeCode = franchiseeID.FranchiseeCode != null ? franchiseeID.FranchiseeCode : "";
                requestList.FranchiseeName = branch.BranchName;
                requestList.Phone = branch.Phone;
                requestList.AlternatePhone = branch.AlternatePhone;
                requestList.Email = branch.Email;
                requestList.Address1 = branch.Address1;
                requestList.Address2 = branch.Address2;
                requestList.Area = branch.Area;
                requestList.District = branch.District;
                requestList.State = branch.State;
                requestList.Country = branch.Country;
                requestList.Pincode = branch.Pincode;
                requestList.Taluk = branchTalukList.FirstOrDefault(x => x.TenantBranchID == renewal.TenantBranchID)?.Taluk ?? string.Empty;
                requestList.PackageType = renewal.PackageType;
                requestList.PackageName = renewal.PackageName;
                requestList.Duration = renewal.Duration;
                requestList.Amount = renewal.Amount;
                requestList.SubscriptionPeroid = string.Concat(renewal.SubscriptionStart, " to ", renewal.SubscriptionEnd);
                if (renewal.PackageType == "Offline")
                {
                    requestList.PaymentDetails = await GetPaymentDetails(renewal.PackageLogID);
                }
                else if (renewal.PaymentType == "Wallet")
                {
                    requestList.PaymentDetails = _franchiseeRepository.PaymentDetails(renewal.PackageLogID);
                }
                renewalRequestLists.Add(requestList);
            }
            return renewalRequestLists;
        }

        /// <summary>
        /// Approve franchisee renewal request
        /// </summary>
        /// <param name="packageLogID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public Guid ApproveRenewalRequest(Guid packageLogID)
        {

            return _franchiseeRepository.ApproveRenewalRequest(packageLogID, _userIdentityInfo.ContactID);
        }

        /// <summary>
        /// Lock franchisee
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param name="isActive"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> LockFranchisee(Guid tenantBranchID, bool isActive)
        {
            ActiveDeactive active = new ActiveDeactive();
            active.ContactID = _franchiseeRepository.GetContactID(tenantBranchID);
            active.Status = isActive;
            string result = await _commonService.LockFranchisee(active);
            int count = _franchiseeRepository.LockFranchisee(tenantBranchID, isActive);
            if (count == 0)
                throw new DawnUserException("Couldn't lock or unlock franchisee");
            return isActive == true ? "Franchisee unlocked successfully" : "Franchisee locked successfully";
            //return string.Empty;
        }

        /// <summary>
        /// Get documents from database
        /// </summary>
        /// <param name="fileId"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public DownloadDocument FileDownLoad(Guid fileId)
        {
            return _franchiseeRepository.FileDownLoad(fileId);
        }
        public List<BaseUrlList> FileDownLoadList(Guid franchiseeRequestID)
        {
            List<BaseUrlList> list = new List<BaseUrlList>();
            DataSet data = _franchiseeRepository.FileDownLoadList(franchiseeRequestID);
            foreach(DataRow item in data.Tables[0].Rows)
            {
                BaseUrlList baseUrl = new BaseUrlList()
                {
                    FileName = item["FileName"]?.ToString() ?? string.Empty,
                    MIMEType = item["MIMEType"]?.ToString() ?? string.Empty,
                    Content = item["Content"] == DBNull.Value ? null : (byte[])item["Content"]
                };
                list.Add(baseUrl);
            }
            return list;
        }

        /// <summary>
        /// Get renewal payment list by admin
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<List<RenewalPaymentList>> GetRenewalPaymentList()
        {
            List<RenewalPaymentList> renewalPaymentLists = new List<RenewalPaymentList>();
            var renewalList = _franchiseeRepository.GetRenewalListByAdmin();
            foreach (var renewal in renewalList)
            {
                var branch = await _commonService.GetBranchDetails(renewal.TenantBranchID);
                //var payments = await _commonService.GetPaymentDetails(renewal.PackageLogID);
                RenewalPaymentList paymentList = new RenewalPaymentList();
                paymentList.TenantBranchID = branch.TenantBranchID;
                paymentList.PackageLogID = renewal.PackageLogID;
                paymentList.FranchiseeCode = renewal.FranchiseeCode;
                paymentList.FranchiseeName = branch.BranchName;
                paymentList.Phone = branch.Phone;
                paymentList.AlternatePhone = branch.AlternatePhone;
                paymentList.Address1 = branch.Address1;
                paymentList.Address2 = branch.Address2;
                paymentList.Area = branch.Area;
                paymentList.District = branch.District;
                paymentList.State = branch.State;
                paymentList.Country = branch.Country;
                paymentList.Pincode = branch.Pincode;
                paymentList.PackageType = renewal.PackageType;
                paymentList.Amount = renewal.Amount;
                if (renewal.PaymentType == "Offline")
                {
                    paymentList.PaymentDetails = await GetPaymentDetails(renewal.PackageLogID);
                }
                else if (renewal.PaymentType == "Wallet")
                {
                    paymentList.PaymentDetails = _franchiseeRepository.PaymentDetails(renewal.PackageLogID);
                }
                renewalPaymentLists.Add(paymentList);
            }
            return renewalPaymentLists;
        }

        /// <summary>
        /// Get franchisee whole periode date
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <param name="tenantBranchID"></param>
        /// <returns></returns>
        public string GetWholePeriodDate(Guid tenantBranchID)
        {
            //string dateFrom = _franchiseeRepository.GetWholePeriod(tenantBranchID);
            //var date = DateTimeOffset.ParseExact(dateFrom,"dd-MM-yyyy HH:mm:ss zzz",CultureInfo.InvariantCulture);
            //return date.ToString("dd-MM-yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture);
            //return date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            
            var dateFrom = _franchiseeRepository.GetWholePeriodDate(tenantBranchID);
            return dateFrom.CreatedDate;
        }

        /// <summary>
        /// Get admin dashboard
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<AdminDashBoard> GetAdminDashboard()
        {
            AdminDashBoard dashBoard = new AdminDashBoard();
            var franchisee = _franchiseeRepository.GetFranchiseeDashboardByAdmin();
            var email = await _commonService.GetMailCount();
            var course = await _commonService.GetCourseSummaryByAdmin();
            var wallet = await _commonService.GetWalletSummary();
            var indent = await _commonService.GetIndentSummaryByAdmin();
            dashBoard.FranchiseePending = franchisee.Pending;
            dashBoard.FranchiseeActive = franchisee.Active;
            dashBoard.FranchiseeExpired = franchisee.Expired;
            dashBoard.FranchiseeLocked = franchisee.Locked;
            dashBoard.RenewalApproval = franchisee.Approval;
            dashBoard.RenewalSubscription = franchisee.Subscription;
            dashBoard.MailPending = email;
            dashBoard.CourseTotal = course.CourseTotal;
            dashBoard.PrintingCertificate = course.PrintCertificate;
            dashBoard.PrintingMarksheet = course.PrintMarksheet;
            dashBoard.DespatchCertificate = course.DespatchCertificate;
            dashBoard.DespatchMarksheet = course.DespatchMarksheet;
            dashBoard.WalletPending = wallet.WalletPending;
            dashBoard.WalletBalance = wallet.WalletBalance;
            dashBoard.IndentExam = indent.IndentExam;
            dashBoard.IndentMaterial = indent.IndentMaterial;
            dashBoard.DespatchMaterial = indent.DespatchMaterial;
            string tenantConn = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            var franchiseeRepo = new FranchiseeRepository(_sqlI18N, tenantConn);
            var contactDetails = franchiseeRepo.GetAdminContact();
            dashBoard.ContactTitle = contactDetails.ContactTitle;
            dashBoard.ContactName = contactDetails.ContactName;
            dashBoard.FranchiseeName = contactDetails.FranchiseeName;
            dashBoard.Area = contactDetails.Area;
            dashBoard.Taluk = contactDetails.Taluk;
            dashBoard.District = contactDetails.District;
            dashBoard.Date = DateTime.Now.ToString("dd-MM-yyyy");
            dashBoard.Day = DateTime.Now.ToString("dddd");
            return dashBoard;
        }

        /// <summary>
        /// Get dashboard by franchisee wise
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<DashboardByFranchisee> GetFranchiseeDashboard()
        {
            DashboardByFranchisee dashboard = new DashboardByFranchisee();
            decimal wallet = await _commonService.GetWalletBalance(_userIdentityInfo.TenantBranchID);
            var course = await _commonService.GetCourseSummaryByFranchisee();
            var indent = await _commonService.GetIndentSummaryByFranchisee();
            var email = await _commonService.GetMailCount();
            var franchisee = _franchiseeRepository.SummaryByFranchisee(_userIdentityInfo.TenantBranchID);
            if (franchisee != null)
            {
                dashboard.ApprovalStartDate = franchisee.ApprovalStartDate;
                dashboard.ApprovalEndDate = franchisee.ApprovalEndDate;
                dashboard.SubscriptionStartDate = franchisee.SubscriptionStartDate;
                dashboard.SubscriptionEndDate = franchisee.SubscriptionEndDate;
                dashboard.ApprovalTotalDays = franchisee.ApprovalTotalDays;
                dashboard.ApprovalCompletedDays = franchisee.ApprovalCompletedDays;
                dashboard.ApprovalRemainingDays = franchisee.ApprovalRemainingDays;
                dashboard.SubscriptionTotalDays = franchisee.SubscriptionTotalDays;
                dashboard.SubscriptionCompletedDays = franchisee.SubscriptionCompletedDays;
                dashboard.SubscriptionRemainingDays = franchisee.SubscriptionRemainingDays;
                dashboard.ContactTitle = franchisee.ContactTitle;
                dashboard.ContactName = franchisee.ContactName;
                dashboard.FranchiseeName = franchisee.FranchiseeName;
                dashboard.Area = franchisee.Area;
                dashboard.Taluk = franchisee.Taluk;
                dashboard.District = franchisee.District;
                dashboard.Date = DateTime.Now.ToString("dd-MM-yyyy");
                dashboard.Day = DateTime.Now.ToString("dddd");
            }
            dashboard.Inbox = email;
            if (wallet > 0)
            {
                dashboard.WalletBalance = wallet;
            }
            if (course != null)
            {
                dashboard.Courses = course.Courses;
                dashboard.ActiveBatch = course.ActiveBatch;
                dashboard.CompletedBatch = course.CompletedBatch;
                dashboard.CurrentStudent = course.CurrentStudent;
                dashboard.AlumniStudent = course.AlumnusStudent;
                dashboard.DiscontinuedStudent = course.DiscontinuedStudent;
                dashboard.CertificatePending = course.CertficatePending;
                dashboard.MarksheetPending = course.MarksheetPending;
            }
            if (indent != null)
            {
                dashboard.MaterialPending = indent.MaterialPending;
                dashboard.ExamPending = indent.ExamPending;
            }
            //dashboard.FeesBalance = await FeesBalanceCount();
            dashboard.FeesBalance = await FeesBalanceSum();
            /*Admin details*/
            string tenantConn = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            var franchiseeRepo = new FranchiseeRepository(_sqlI18N, tenantConn);
            var contactDetails = franchiseeRepo.GetAdminContact();
            dashboard.AdminDetails = contactDetails;
            dashboard.OwnerImage = _franchiseeRepository.GetOwnerImage(_userIdentityInfo.TenantBranchID);
            return dashboard;
        }

        /// <summary>
        /// Reject paid request
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <param name="reject"></param>
        /// <returns></returns>
        public string RejectPaidRequest(RejectPaidRequest reject)
        {
            int franchisee = _franchiseeRepository.RejectPaidRequest(reject, _userIdentityInfo.ContactID);
            return franchisee > 0 ? "Paid request rejected" : throw new DawnUserException("Couldn't reject paid request");
        }

        public string AddVideo(AddVideos add)
        {
            if (add == null)
                throw new DawnUserException("Invalid data.");
            int count = _franchiseeRepository.CheckShorOrderExists(add);
            if (count > 0)
                throw new DawnUserException("The sort order already exists");
            int result = _franchiseeRepository.AddVideo(add, _userIdentityInfo.ContactID);
            if (result > 0)
            {
                return "Video uploaded successfully";
            }
            else
            {
                throw new DawnUserException("Couldn't upload video");
            }
        }

        public string EditVideo(EditVideo edit)
        {
            if (edit == null)
                throw new DawnUserException("Invalid data");
            int sortOrder = _franchiseeRepository.GetSortOrder(edit.GuideVideoID);
            if (_franchiseeRepository.CheckSortOrder(edit) && edit.SortOrder != sortOrder)
                throw new DawnUserException("The sortorder already exists");
            int result = _franchiseeRepository.EditVideo(edit, _userIdentityInfo.ContactID);
            return result > 0 ? "Edit video successfully" : throw new DawnUserException("Couldn't edit video");
        }

        public GetVideos GetGuideVideos(string mobile_app)
        {
            GetVideos videos = new GetVideos();
            string title = "Title";
            string description = "Description";
            videos.Title = title;
            videos.Description = description;
            videos.Videos = _franchiseeRepository.GetGuideVideos(mobile_app);
            return videos;
        }

        public async Task<List<MblFranchisee>> GetFranchisees(MblFilter filter)
        {
            var franchisees = _franchiseeRepository.GetFranchisees(filter);
            List<MblFranchisee> list = new List<MblFranchisee>();
            foreach (var franchisee in franchisees)
            {
                MblFranchisee mbl = franchisee.Mapper<MblFranchisee>();
                mbl.Courses = await _commonService.GetMblCourseList(franchisee.TenantBranchID);
                mbl.OwnerImage = _franchiseeRepository.GetOwner(franchisee.TenantBranchID);
                list.Add(mbl);
            }
            return list;
        }

        public async Task<List<MlbRenewalList>> MblRenewalList()
        {
            List<MlbRenewalList> mlbRenewals = new List<MlbRenewalList>();
            var list = _franchiseeRepository.MblRenewalList(_userIdentityInfo.TenantBranchID);
            foreach (var mbl in list)
            {
                MlbRenewalList mlbRenewal = mbl.Mapper<MlbRenewalList>();
                var payment = await _commonService.GetFranchiseePayments(mbl.PackageLogID);
                if (mbl.PaymentType == "Offline")
                {
                    mlbRenewal.Payments = payment;
                }
                else if (mbl.PaymentType == "Wallet")
                {
                    mlbRenewal.Payments = _franchiseeRepository.PaymentList(mbl.PackageLogID);
                }
                mlbRenewals.Add(mlbRenewal);
            }
            return mlbRenewals;
        }

        public async Task<List<RankList>> GetStateFranchiseeRank(StateRankFilter filter)
        {
            List<RankList> rankLists = new List<RankList>();
            List<PurchaseAmount> amount = new List<PurchaseAmount>();
            if (filter.Type.ToConditionFormat() == "ADMISSIONWISE")
            {
                string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
                CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
                amount = commonRepository.GetAdmissionFeesList(filter.Year);
            }
            else if (filter.Type.ToConditionFormat() == "TRANSACTIONWISE")
            {
                string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Inventory");
                CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
                amount = commonRepository.GetPurchaseAmount(filter.Year);
            }
            else
            {
                List<PurchaseAmount> purchaseAmounts = new List<PurchaseAmount>();
                List<PurchaseAmount> admissionFees = new List<PurchaseAmount>();
                string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Inventory");
                string _connectionStringPay = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
                CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
                purchaseAmounts = commonRepository.GetPurchaseAmount(filter.Year);
                CommonRepository commonRepositoryP = new CommonRepository(_sqlI18N, _connectionStringPay);
                admissionFees = commonRepositoryP.GetAdmissionFeesList(filter.Year);
                amount = admissionFees;
            }
            DataTable data = amount.ToDataTable();
            rankLists = _franchiseeRepository.GetStateFranchiseeRank(data, filter);
            rankLists = await GetTenantName(rankLists);
            if (amount != null && amount.Count > 0 && filter.Type.ToConditionFormat() != "ADMISSIONWISE" && filter.Type.ToConditionFormat() != "TRANSACTIONWISE")
            {
                var rankedAmounts = amount.OrderByDescending(x => x.Total).ToList();
                var rankMap = new Dictionary<Guid, int>();
                int rank = 1;
                foreach (var amt in rankedAmounts)
                {
                    var item = rankLists.FirstOrDefault(x => x.TenantBranchID == amt.TenantBranchID);
                    if (item != null)
                    {
                        item.PurchaseRank = rank++;
                    }
                }
                foreach (var item in rankLists.Where(x => x.PurchaseRank == 0))
                {
                    item.PurchaseRank = 0;
                }
            }

            return rankLists;
        }
        public async Task<List<RankList>> GetTenantName(List<RankList> rankLists)
        {
            List<RankList> rankList = new List<RankList>();
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            foreach (var rank in rankLists)
            {
                FranchiseeRepository franchiseeRepository = new FranchiseeRepository(_sqlI18N, _connectionString);
                {
                    rank.CentreName = franchiseeRepository.GetCenterName(rank.TenantBranchID);
                }
                rankList.Add(rank);
            }
            return rankList;
        }

        public async Task<List<RankList>> GetDistrictFranchiseeRank(DistrictRankFilter filter)
        {
            List<RankList> rankLists = new List<RankList>();
            List<PurchaseAmount> amount = new List<PurchaseAmount>();
            List<PurchaseAmount> purchaseAmounts = new List<PurchaseAmount>();
            List<PurchaseAmount> admissionFees = new List<PurchaseAmount>();
            if (filter.Type.ToConditionFormat() == "ADMISSIONWISE")
            {
                string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
                CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
                amount = commonRepository.GetAdmissionFeesList(filter.Year);
            }
            else if (filter.Type.ToConditionFormat() == "TRANSACTIONWISE")
            {
                string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Inventory");
                CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
                amount = commonRepository.GetPurchaseAmount(filter.Year);
            }
            else
            {
                string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Inventory");
                string _connectionStringPay = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
                CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
                amount = commonRepository.GetPurchaseAmount(filter.Year);
                CommonRepository commonRepositoryP = new CommonRepository(_sqlI18N, _connectionStringPay);
                admissionFees = commonRepositoryP.GetAdmissionFeesList(filter.Year);
                amount = admissionFees;
            }
            DataTable data = amount.ToDataTable();
            rankLists = _franchiseeRepository.GetDistrictFranchiseeRank(data, filter);
            rankLists = await GetTenantName(rankLists);

            if (amount != null && amount.Count > 0 && filter.Type.ToConditionFormat() != "ADMISSIONWISE" && filter.Type.ToConditionFormat() != "TRANSACTIONWISE")
            {
                var rankedAmounts = amount.OrderByDescending(x => x.Total).ToList();
                var rankMap = new Dictionary<Guid, int>();
                int rank = 1;
                foreach (var amt in rankedAmounts)
                {
                    var item = rankLists.FirstOrDefault(x => x.TenantBranchID == amt.TenantBranchID);
                    if (item != null)
                    {
                        item.PurchaseRank = rank++;
                    }
                }
                foreach (var item in rankLists.Where(x => x.PurchaseRank == 0))
                {
                    item.PurchaseRank = 0;
                }
            }
            return rankLists;
        }

        public async Task<List<RankList>> GetStateFranchiseeAchiever(StateAchieversFilter filter)
        {
            //List<PurchaseAmount> amount = new List<PurchaseAmount>();
            //string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Inventory");
            //CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            //amount = commonRepository.GetPurchaseAmount(filter.Year);
            //DataTable data = amount.ToDataTable();
            //return _franchiseeRepository.GetStateFranchiseeAchiever(data, filter);

            List<RankList> rankLists = new List<RankList>();
            List<PurchaseAmount> purchaseAmounts = new List<PurchaseAmount>();
            List<PurchaseAmount> admissionFees = new List<PurchaseAmount>();

            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Inventory");
            string _connectionStringPay = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");

            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            purchaseAmounts = commonRepository.GetPurchaseAmount(filter.Year);

            CommonRepository commonRepositoryP = new CommonRepository(_sqlI18N, _connectionStringPay);
            admissionFees = commonRepositoryP.GetAdmissionFeesList(filter.Year);

            var mergedAmounts = purchaseAmounts
                .Concat(admissionFees)
                .GroupBy(a => a.TenantBranchID)
                .Select(g => new PurchaseAmount
                {
                    TenantBranchID = g.Key,
                    Total = g.Sum(x => x.Total)
                })
                .ToList();
            DataTable data = mergedAmounts.ToDataTable();
            rankLists = _franchiseeRepository.GetStateFranchiseeAchiever(data, filter);
            rankLists = await GetTenantName(rankLists);
            return rankLists;
        }

        public async Task<List<RankList>> GetDistrictFranchiseeAchiever(DistrictAchieversFilter filter)
        {
            //List<PurchaseAmount> amount = new List<PurchaseAmount>();
            //string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Inventory");
            //CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            //amount = commonRepository.GetPurchaseAmount(filter.Year);
            //DataTable data = amount.ToDataTable();
            //return _franchiseeRepository.GetDistrictFranchiseeAchiever(data, filter);

            List<RankList> rankLists = new List<RankList>();
            List<PurchaseAmount> purchaseAmounts = new List<PurchaseAmount>();
            List<PurchaseAmount> admissionFees = new List<PurchaseAmount>();

            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Inventory");
            string _connectionStringPay = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");

            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            purchaseAmounts = commonRepository.GetPurchaseAmount(filter.Year);

            CommonRepository commonRepositoryP = new CommonRepository(_sqlI18N, _connectionStringPay);
            admissionFees = commonRepositoryP.GetAdmissionFeesList(filter.Year);

            var mergedAmounts = purchaseAmounts
                .Concat(admissionFees)
                .GroupBy(a => a.TenantBranchID)
                .Select(g => new PurchaseAmount
                {
                    TenantBranchID = g.Key,
                    Total = g.Sum(x => x.Total)
                })
                .ToList();

            DataTable data = mergedAmounts.ToDataTable();
            rankLists = _franchiseeRepository.GetDistrictFranchiseeAchiever(data, filter);
            rankLists = await GetTenantName(rankLists);
            return rankLists;
        }

        public async Task<FranchiseeRankStatus> GetFranchiseeRankStatus()
        {
            FranchiseeRankStatus status = new FranchiseeRankStatus();
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var admisstionamount = commonRepository.GetAdmissionFeesList(DateTime.Now.Year);
            var purchaseAdmount = await _commonService.GetPurchaseAmount();
            return _franchiseeRepository.GetFranchiseeRankStatus(purchaseAdmount, admisstionamount, _userIdentityInfo.TenantBranchID);
        }

        public string CreateNotice(CreateNotice create)
        {
            int result = _franchiseeRepository.CreateNoticeBoard(create, _userIdentityInfo.TenantBranchID, _userIdentityInfo.ContactID);
            if (result > 0)
            {
                return "Created successfully";
            }
            else
            {
                throw new DawnUserException("Couldn't create");
            }
        }

        public GetNotice GetNoticeBoard()
        {
            return _franchiseeRepository.GetNoticeBoard(_userIdentityInfo.TenantBranchID);
        }

        public HelpAndSupport GetHelpAndSupport()
        {
            return _franchiseeRepository.GetHelpAndSupport(_userIdentityInfo.TenantBranchID);
        }

        public async Task<string> UploadStudentFeedback(UploadStudentFeedback file)
        {
            DTOStudentFeedback feedback = new DTOStudentFeedback();
            if (file == null || file.File.Length == 0)
                throw new DawnUserException("No file uploaded.");
            string extension = Path.GetExtension(file.File.FileName).ToLower();
            string fileName = Guid.NewGuid().ToString() + extension;
            feedback.FileName = fileName;
            feedback.Title = file.Title;
            feedback.TenantBranchID = _userIdentityInfo.TenantBranchID;
            // Get the application's root path
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/videos", fileName);
            if (file.File.Length > 0)
            {
                using (var stream = new FileStream(rootPath, FileMode.Create))
                {
                    await file.File.CopyToAsync(stream);
                }
            }
            int count = _franchiseeRepository.UploadStudentFeedback(feedback, _userIdentityInfo.ContactID);
            if (count > 0)
                return "File uploaded successfully.";
            else
            {
                var dlt = DeleteFile(fileName);
                throw new DawnUserException("Couldn't upload file");
            }
        }

        public string ApproveFeedback(ApproveFeedback approve)
        {
            int sortOrder = _franchiseeRepository.GetSortOrderNum(approve.TenantBranchID);
            if (sortOrder != 0)
            {
                sortOrder = _franchiseeRepository.GetNewSortOrderNum(approve.TenantBranchID);
            }
            sortOrder = sortOrder != 0 ? ++sortOrder : 1;
            //sortOrder = sortOrder++;
            int count = _franchiseeRepository.ApproveFeedback(approve, _userIdentityInfo.ContactID, sortOrder);
            if (count > 0)
            {
                var res = DeleteFile(approve.FileName);
                return "Request approved successfully.";
            }
            else
            {
                throw new DawnUserException("Couldn't approve.");
            }
        }

        public string RejectFeedback(RejectStudentFeedback reject)
        {
            int count = _franchiseeRepository.RejectFeedback(reject, _userIdentityInfo.ContactID);
            if (count > 0)
            {
                var dlt = DeleteFile(reject.FileName);
                return "Request rejected.";
            }
            else
            {
                throw new DawnUserException("Couldn't reject");
            }
        }

        public string DeleteFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new DawnUserException("File name is not provided.");
            var filePath = Path.Combine(_environment.WebRootPath, "videos", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                throw new DawnUserException("File not found.");
            }
            try
            {
                System.IO.File.Delete(filePath);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message);
            }
            return "File deleted.";
        }

        public List<GetStudentFeedback> GetFeedbackByAdmin()
        {
            return _franchiseeRepository.GetFeedbackByAdmin();
        }

        public async Task<string> BankCreation(List<GetBanks> banks, Guid adminContactID, Guid branchID)
        {
            string result = string.Empty;
            foreach (var bank in banks)
            {
                AddBankAccount add = bank.Mapper<AddBankAccount>();
                add.AccountID = Guid.NewGuid();
                add.BranchID = branchID;
                add.AdminContactID = adminContactID;
                add.OpeningBalance = 0;
                add.JournalDate = DateTime.Now.ToString("dd-MM-yyyy");
                result = await _commonService.AddBank(add);
            }
            return result;
        }

        public async Task<bool> FranchiseeToAdmin(List<FranchiseePayments> payments, Guid giverBranchID, Guid adminBranchID, Guid giverContactID, Guid recieverContactID, Guid paymentID)
        {
            bool journalPosting = false;
            var branchBanks = await _commonService.GetBranchBankList(giverBranchID);
            foreach (var payment in payments)
            {
                var bank = branchBanks.Where(x => x.LedgerName.ToUpper() == payment.LedgerName.ToUpper()).FirstOrDefault();

                NewPaymentDetails details = new NewPaymentDetails();
                details.PaymentModeID = payment.PaymodeID;
                details.PayModeType = payment.PaymodeType;
                details.PayModeValue = payment.TransactionAmount;
                details.Amount = payment.Amount;
                details.FromLedgerID = payment.PaymodeID;
                if (payment.ToLedgerName.ToConditionFormat() == "UNDEPOSITEDCASH")
                {
                    details.FromLedgerName = payment.ToLedgerName;
                }
                else
                {
                    details.FromLedgerName = bank != null ? bank.LedgerName : "";
                }
                details.ToLedgerID = payment.ToLedgerID;
                details.ToLedgerName = payment.ToLedgerName;
                details.TransactionDate = payment.TransactionDate;
                details.TransactionNo = payment.TransactionNo;
                var paymentDetail = JsonConvert.SerializeObject(details);
                BranchToAdmin journal = new BranchToAdmin();
                journal.GiverBranchID = giverBranchID;
                journal.ReceiverBranchID = adminBranchID;
                journal.BaseAmount = payment.Amount;
                journal.Narration = payment.TransactionName;
                journal.GiverContactID = giverContactID;
                journal.ReceiverContactID = recieverContactID;
                journal.TransactionAmount = payment.Amount;
                journal.PaymentID = paymentID;
                journal.PaymentDetail = paymentDetail;
                if (payment.ToLedgerName.ToConditionFormat() == "UNDEPOSITEDCASH")
                {
                    details.FromLedgerName = payment.ToLedgerName;
                }
                else
                {
                    details.FromLedgerName = bank != null ? bank.LedgerName : "";
                }
                journal.ToLedgerID = payment.ToLedgerID;
                journalPosting = await _commonService.FranchiseeToAdmin(journal);
            }
            return journalPosting;
        }

        public List<GetMobileApp> GetMobileApps()
        {
            return _franchiseeRepository.GetMobileApps("MOBILE APP");
        }

        public List<GetMobileApp> GetMobileShowIn()
        {
            return _franchiseeRepository.GetMobileApps("SHOW IN");
        }

        public List<GuideVideos> GuideVideos()
        {
            return _franchiseeRepository.GuideVideos();
        }

        public async Task<List<Ledger>> GetAdminBankList()
        {
            return await _commonService.GetAdminBankList();
        }

        public string DeleteGuideVideo(Guid guideVideoID)
        {
            int result = _franchiseeRepository.DeleteGuideVideo(guideVideoID, _userIdentityInfo.ContactID);
            return result > 0 ? "Video deleted." : throw new DawnUserException("Couldn't delete");
        }

        public List<GuideVideos> GetFeedbackVideo()
        {
            return _franchiseeRepository.GetGuideVideo("Franchisee", "Feedback");
        }

        public List<GuideVideos> GetWelcomeVideo()
        {
            return _franchiseeRepository.GetGuideVideo("Student", "Welcome");
        }

        public List<GuideVideos> GetPurposeVideo()
        {
            return _franchiseeRepository.GetGuideVideo("Student", "Purpose");
        }
        public List<GuideVideos> GetStudentFeedbackVideo()
        {
            return _franchiseeRepository.GetStudentFeedback(_userIdentityInfo.TenantBranchID, "Student", "Feedback");
        }

        private async Task<string> GetConnectionString(Guid tenantId, string serviceName)
        {
            TenantServiceInfo tenantServiceInfo = new()
            {
                ServiceName = serviceName,
                TenantID = tenantId
            };
            dynamic dbAdminAPIResult = await _dawnHttpClient.PostAsync("dbadmin/get-tenant-constr-on-reg", tenantServiceInfo, false);
            string constring = dbAdminAPIResult.Code == 200 ? dbAdminAPIResult.Data.ToString() : "";
            return constring.ToDecrypt();
        }

        public async Task<DocumentResponce> ImageDocumentUpload(ImageDocument imageUpload)
        {
            return await _commonService.DocumentUpload(imageUpload);
        }

        public async Task<List<PaymentDetails>> GetPaymentDetails(Guid indentID)
        {
            List<PaymentDetails> paymentDetails = new List<PaymentDetails>();
            var payments = await _commonService.GetPaymentDetails(indentID);
            foreach (var item in payments)
            {
                PaymentDetails payment = new PaymentDetails();
                payment.TransactionDate = item.TransactionDate;
                payment.Amount = item.Amount;
                payment.Summary = item.Summary;
                payment.PayMode = item.PayMode;
                var document = await _commonService.GetDocumentID(indentID, item.PayMode);
                payment.DocumentID = Guid.Parse(document);
                paymentDetails.Add(payment);
            }
            return paymentDetails;
        }

        public List<FranchiseeBanks> GetFranchiseeBanks(Guid franchiseeRequestID)
        {
            return _franchiseeRepository.GetFranchiseeBanks(franchiseeRequestID);
        }

        public string[] GetLevelStatus()
        {
            string[] level = new string[] { "State Level", "District Level" };
            return level;
        }

        public string[] GetTypeStatus()
        {
            string[] level = new string[] { "Admission Wise", "Transaction Wise", "Both Admission and Transaction Wise" };
            return level;
        }

        public IEnumerable<GetMobileApp> GetRelationship()
        {
            return _franchiseeRepository.GetRelationship();
        }

        public async Task<OTPVerifyResult> OTPVerify(OTPVerify otpVerify)
        {
            OTPVerifyResult oTPVerifyResult = new OTPVerifyResult();
            try
            {
                VerifyOTP verifyEmailOTP = new VerifyOTP();
                verifyEmailOTP.OTPTransID = otpVerify.EmailOTPTransID;
                verifyEmailOTP.OTPCode = otpVerify.EmailOTPCode;
                verifyEmailOTP.OTPType = $"Registration Verification";
                oTPVerifyResult.IsEmailVerified = await _commonService.IsOTPChecked(otpVerify.EmailOTPTransID);
                if (!oTPVerifyResult.IsEmailVerified)
                    oTPVerifyResult.IsEmailVerified = await OTPVerify(verifyEmailOTP);

                if (oTPVerifyResult.IsEmailVerified)
                {
                    oTPVerifyResult.IsOTPVerified = true;
                    oTPVerifyResult.Message = OTPMessage.OTPVerified;
                }
                else if (!oTPVerifyResult.IsEmailVerified)
                {
                    oTPVerifyResult.IsOTPVerified = false;
                    oTPVerifyResult.Message = OTPMessage.EmailOTPVerifiedError;
                }
                else if (oTPVerifyResult.IsEmailVerified)
                {
                    oTPVerifyResult.IsOTPVerified = false;
                    oTPVerifyResult.Message = OTPMessage.SMSOTPVerifiedError;
                }
                else
                    oTPVerifyResult.Message = OTPMessage.OTPVerifiedError;

                return oTPVerifyResult;
            }
            catch (Exception ex)
            {
                return oTPVerifyResult;
            }
        }

        private async Task<bool> OTPVerify(VerifyOTP otpVerify)
        {
            Guid otpVerificationID = await _commonService.OTPVerify(otpVerify);
            return otpVerificationID == Guid.Empty ? false : true;
        }

        public async Task<OTPResult> EmailOTPRequest(string email)
        {
            EmailOTPRequest emailOTPRequest = new EmailOTPRequest();
            emailOTPRequest.MailAddress = email;
            emailOTPRequest.OTPType = $"Registration Verification";
            emailOTPRequest.MessageParameter = new List<string> { "TCDS Computer Education", "", "OTPCode", "" };
            EmailOTPSentResult result = await _commonService.SendEmailOTPRequest(emailOTPRequest);
            OTPResult otpResult = new OTPResult();
            otpResult.OTPTransID = result.OTPTransID;
            otpResult.ValidityTime = result.ValidityTime;
            return otpResult;
        }

        public async Task<OTPResult> RegistrationOTPRequest(RegistrationOTPRequest otpRequest)
        {
            await CheckRequiredForOTPSend(otpRequest);
            OTPResult otpSendResult = new OTPResult();
            var emailResult = await EmailOTPRequest($"{otpRequest.Email}");
            otpSendResult.OTPTransID = emailResult.OTPTransID;
            otpSendResult.ValidityTime = emailResult.ValidityTime;
            return otpSendResult;
        }

        private async Task CheckRequiredForOTPSend(RegistrationOTPRequest otpRequest)
        {
            if (await _commonService.IsEmailExistCheck(otpRequest.Email))
                throw new DawnUserException(OTPMessage.EmailExists);
        }

        private async Task CheckRequiredForOTPValidation(Guid emailOTPTransID)
        {
            //Check sms otp verified
            if (!await _commonService.IsOTPVerified(emailOTPTransID))
                throw new DawnException("Email OTP is not verified");
        }

        public async Task<string> SendSuccessResponse(string email, string password, string contactName)
        {
            EmailSendRequest emailSend = new EmailSendRequest();
            emailSend.MailAddress = email;
            emailSend.EmailType = "RegistrationCompleted";
            List<string> temp = new List<string>();
            temp.Add(contactName);
            temp.Add(email);
            temp.Add(password);
            //emailSend.MessageParameter = new List<string> { contactName, email, password };
            emailSend.MessageParameter = temp;
            string result = await _commonService.RegCompletedMail(emailSend);
            return result;
        }

        public async Task<List<PaymentHistoryMaster>> PaymentHistory(Guid packageLogID)
        {
            List<PaymentHistoryMaster> payments = new List<PaymentHistoryMaster>();
            DataSet dsPayments = _franchiseeRepository.FranchiseePaymentHistory(packageLogID);
            var paymentMaster = dsPayments.Tables[0].DataTablesToList<PaymentHistoryMaster>();
            var paymentTrans = dsPayments.Tables[1].DataTablesToList<PaymentHistoryTrans>();
            foreach (var item in paymentMaster)
            {
                PaymentHistoryMaster payment = item.Mapper<PaymentHistoryMaster>();
                var transList = paymentTrans.Where(x => x.FranchiseePaymentID == item.FranchiseePaymentID).ToList();
                foreach (var trans in transList)
                {
                    string documentID = await _commonService.GetDocumentID(trans.FranchiseePaymentTransID, trans.PayMode);
                    trans.DocumentID = Guid.Parse(documentID);
                    payment.Payments.Add(trans);
                }
                payments.Add(payment);
            }
            return payments;
        }

        public IEnumerable<TalukList> GetTalukList(string district)
        {
            return _franchiseeRepository.GetTalukList(district);
        }

        public async Task<List<FeesBalanceCount>> GetPaidFees()
        {
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var paidFees = commonRepository.GetPaidFees();
            return paidFees;
        }

        public async Task<decimal> FeesBalanceSum()
        {
            string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Admission");
            var commonRepository = new CommonRepository(_sqlI18N, connectionString);
            var netFees = commonRepository.GetNetFees(_userIdentityInfo.TenantBranchID);
            var paidFees = await GetPaidFees();
            var feesWithBalance = from n in netFees
                                  join p in paidFees on n.BatchMappingID equals p.BatchMappingID
                                  select new
                                  {
                                      FeesBalance = n.NetCourseFees - p.PaidFees
                                  };
            decimal totalFeesBalance = feesWithBalance.Sum(f => f.FeesBalance);
            return totalFeesBalance.ToDecimal();
        }


        public string[] GetAccountType()
        {
            string[] type = new string[] { "Current Account", "Savings Account" };
            return type;
        }

        public GetApprovalCertificate GetApprovalCertificate(Guid tenantBranchID)
        {
            var response = _franchiseeRepository.GetApprovalCertificate(tenantBranchID);
            var document = FileDownLoad(response.DocumentID);
            response.ContentUrl = Convert.ToBase64String(document.Content);
            return response;
        }

        public string EditFranchiseeCode(EditFranchiseeCode edit)
        {
            if (edit == null)
                throw new DawnUserException("Invalid data.");
            if (_franchiseeRepository.CheckCodeExists(edit))
                throw new DawnUserException("Franchisee ID already exists.");
            int count = _franchiseeRepository.EditFranchiseeCode(edit, _userIdentityInfo.ContactID);
            return count > 0 ? "Franchisee ID updated successfully." : throw new DawnUserException("Couldn't update franchisee ID.");
        }
        public IEnumerable<StateList> GetStateList(string country)
        {
            return _franchiseeRepository.GetStateList(country);
        }
        public IEnumerable<DistrictList> GetDistrictList(string state)
        {
            return _franchiseeRepository.GetDistrictList(state);
        }
        public int GetCourierCharges()
        {
            return _franchiseeRepository.GetCourierCharges();
        }
        public int GetIDCardCharges()
        {
            return _franchiseeRepository.GetIDCardCharges();
        }
        public string UpdateIDCardCharges(int amount)
        {
            return _franchiseeRepository.UpdateCharges(amount, "IDCARD_CHARGES") > 0 ? "ID Card charge updated Successful !!!" : throw new Exception("Something went wrong");
        }
        public string UpdateCourierCharges(int amount)
        {
            return _franchiseeRepository.UpdateCharges(amount,"COURIER_CHARGES") > 0 ? "Courier charge updated Successful !!!" : throw new Exception("Something went wrong");
        }
        public string IDCardCourierChargeUpdate(ChargeUpdate amount)
        {
            return _franchiseeRepository.IDCardCourierChargesUpdate(amount) > 0 ? "Charge updated Successful !!!" : throw new Exception("Something went wrong");
        }

        public string EditFranchiseePersonalDetails(EditFranchiseeCodeData edit)
        {
            if (edit == null)
                throw new DawnUserException("Invalid data.");
         
            if(!_franchiseeRepository.EditFranchiseeContactInfo(edit, _userIdentityInfo.ContactID))
            {
                throw new DawnUserException("Failed to update contact details.");
            }
            return "Contact details updated successfully.";

        }

        public string EditFranchiseeDetails(EditFranchiseeCodeData edit)
        {
            if (edit == null)
                throw new DawnUserException("Invalid data.");

            if (!_franchiseeRepository.EditFranchiseeInfo(edit, _userIdentityInfo.ContactID))
            {
                throw new DawnUserException("Failed to update franchisee details");
            }
            return "Franchisee details updated successfully.";
        
        }
        #endregion
    }
}