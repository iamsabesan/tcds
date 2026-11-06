using Dawn.Utilities.Exceptions;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.Logger;
using EntitiesDTO.Admin;
using EntitiesDTO.Course;
using EntitiesDTO.Material;
using EntitiesDTO.OTP;
using EntitiesDTO.Payment;
using EntitiesDTO.Student;
using EntitiesDTO.Users;
using EntitiesDTO.Utilities;
using Frameworks.Helper;
using Newtonsoft.Json;
using System.Data;
using static EntitiesDTO.Utilities.DawnServiceURL;

namespace CommonServices
{
    public class CommonService
    {
        private readonly IDawnHttpClient _dawnHttpClient;
        public CommonService(IDawnHttpClient dawnHttpClient)
        {
            _dawnHttpClient = dawnHttpClient;
        }
        public async Task<string> CreateFranchisee(TenantContactUserWrite createNewUser)
        {
            try
            {
                dynamic contactAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.UserService.CreateFranchisee, createNewUser);
                string message = contactAPIResult.Message;
                return contactAPIResult.Code == 201 ? contactAPIResult.Data : throw new Exception(message);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> CreateStudent(TenantContactUserWrite createNewUser)
        {
            try
            {
                dynamic contactAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.UserService.CreateStudent, createNewUser);
                string message = contactAPIResult.Message;
                string resultData = contactAPIResult.Data.ToString();
                return contactAPIResult.Code == 201 ? resultData : throw new DawnUserException(message);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }

        /// <summary>
        /// Create contact
        /// </summary>
        /// <param name="createNewContact"></param>
        /// <returns></returns>
        public async Task<string> CreateContact(ContactCreate createNewContact)
        {
            try
            {
                dynamic contactAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.ContactService.CreateContact, createNewContact);
                string message = contactAPIResult.Message;
                return contactAPIResult.Code == 201 ? contactAPIResult.Data : throw new Exception(message);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Create Branch
        /// </summary>
        /// <param name="createNewBranch"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> CreateBranch(BranchCreate createNewBranch)
        {
            var homeBranch = await GetHomeBranchDetails();
            createNewBranch.CountryID = homeBranch.CountryID;
            createNewBranch.Currency = homeBranch.Currency;
            createNewBranch.CurrencyCode = homeBranch.CurrencyCode;
            createNewBranch.CurrencySymbol = homeBranch.CurrencySymbol;
            createNewBranch.DateTimeFormat = homeBranch.DateTimeFormat;
            createNewBranch.DateFormat = homeBranch.DateFormat;
            createNewBranch.TimeFormat = homeBranch.TimeFormat;
            createNewBranch.TimeZoneID = homeBranch.TimeZoneID;
            createNewBranch.LanguageCulture = homeBranch.LanguageCulture;
            createNewBranch.logo = string.Empty;
            dynamic branchAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.TenantBranchService.CreateBranchURL, createNewBranch);
            string message = branchAPIResult.Message;
            return branchAPIResult.Code == 201 ? branchAPIResult.Data : throw new DawnUserException(message);
        }

        /// <summary>
        /// Get home branch details
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="25-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<GetHomeBranchDetails> GetHomeBranchDetails()
        {
            dynamic branchAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.TenantBranchService.GetHomeBranchDetails);
            string message = branchAPIResult.Message;
            string branchAPIData = branchAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<GetHomeBranchDetails>(branchAPIData);
            return branchAPIResult.Code == 200 ? branchData : throw new Exception(message);
        }

        /// <summary>
        /// Create Contact Address
        /// </summary>
        /// <param name="contactAddress"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> CreateContactAddress(ContactAddress contactAddress)
        {
            try
            {
                dynamic contactAddressAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.ContactService.AddressUpdate, contactAddress);
                if (Convert.ToInt16(contactAddressAPIResult.Code) != 201)
                    throw new DawnUserException("Address updation failed");
                string message = contactAddressAPIResult.Message;
                return contactAddressAPIResult.Code == 201 ? message : throw new Exception(message);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> CreateStudentAddress(StudentAddress studentAddress)
        {
            try
            {
                dynamic contactAddressAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.ContactService.AddressUpdate, studentAddress);
                string message = contactAddressAPIResult.Message;
                return contactAddressAPIResult.Code == 201 ? message : throw new Exception(message);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Get tenant branch and contact details
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<DataSet> GetTenantBranchAndContact()
        {
            dynamic branchAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.TenantBranchService.TenantBranchContact);
            string message = branchAPIResult.Message;
            string branchAPIData = branchAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<DataSet>(branchAPIData);
            return branchAPIResult.Code == 200 ? branchData! : throw new Exception(message);
        }

        /// <summary>
        /// Get material name list
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<MaterialNames>> GetMaterialNameList()
        {
            dynamic materialAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.InventoryService.AllMaterialList);
            string message = materialAPIResult.Message;
            string branchAPIData = materialAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<List<MaterialNames>>(branchAPIData);
            return materialAPIResult.Code == 200 ? branchData! : throw new Exception(message);
        }

        public async Task<List<GetExamDate>> GetExamDate(Guid examIndentID)
        {
            dynamic inventoryAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.InventoryService.GetExamDate}{examIndentID}");
            string message = inventoryAPIResult.Message;
            string inventoryAPIData = inventoryAPIResult.Data.ToString();
            var inventoryData = JsonConvert.DeserializeObject<List<GetExamDate>>(inventoryAPIData);
            return inventoryAPIResult.Code == 200 ? inventoryData! : throw new Exception(message);
        }

        /// <summary>
        /// Rollback contact
        /// </summary>
        /// <param name="contactID"></param>
        /// <returns></returns>
        public async Task<string> RollBackUserAndBranch(string userID, string branchID)
        {
            try
            {
                //dynamic contactAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.ContactService.RemoveContact}{contactID}");
                await RollBackUser(userID);
                await RollBackBranch(branchID);
                //string message = contactAPIResult.Message;
                //return contactAPIResult.Code == 200 ? contactAPIResult.Data : throw new Exception(message);
                string message = "User rollbacked successfully!";
                return message;
            }
            catch
            {
                return "User rollback failed.";
            }
        }
        public async Task<string> UserRollBack(string userID)
        {
            try
            {
                await RollBackUser(userID);
                return "User rollbacked successfully!";
            }
            catch
            {
                return "User rollback failed.";
            }
        }

        public async Task<string> RollBackContactByID(string contactID)
        {
            try
            {
                dynamic contactAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.ContactService.RemoveContact}{contactID}");
                string message = contactAPIResult.Message;
                return contactAPIResult.Code == 200 ? contactAPIResult.Data : throw new Exception(message);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Rollback user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<string> RollBackUser(string userID)
        {
            try
            {
                dynamic contactAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.UserService.RollBackUser}{userID}");
                //await RollBackBranch(branchID);
                string message = contactAPIResult.Message;
                return contactAPIResult.Code == 200 ? contactAPIResult.Data : throw new Exception(message);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Rollback contact address
        /// </summary>
        /// <param name="contactID"></param>
        /// <returns></returns>
        public async Task<string> RollBackContactAddress(string contactID, string userID, string branchID, string addressID)
        {
            try
            {
                dynamic contactAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.ContactService.RollBackContactAddress}{contactID}");
                await RollBackUserAndBranch(userID, branchID);
                string message = contactAPIResult.Message;
                return contactAPIResult.Code == 200 ? contactAPIResult.Data : throw new Exception(message);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Rollback branch
        /// </summary>
        /// <param name="branchID"></param>
        /// <returns></returns>
        public async Task<string> RollBackBranch(string branchID)
        {
            try
            {
                dynamic branchAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.TenantBranchService.RollBackTenantBranch}{branchID}");
                string message = branchAPIResult.Message;
                return branchAPIResult.Code == 200 ? branchAPIResult.Data : throw new Exception(message);
            }
            catch
            {
                return null;
            }
        }
        public async Task<string> GetUserID(Guid contactID)
        {
            try
            {
                dynamic contactAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.UserService.GetUserID}{contactID}");
                string message = contactAPIResult.Message;
                return contactAPIResult.Code == 200 ? contactAPIResult.Data : throw new Exception(message);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get active franchisee contact info
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<ActiveFranchiseeContact>> ActiveFranchiseeContact(Guid? contactID)
        {
            dynamic branchAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.TenantBranchService.GetActiveFranchisee}{contactID}");
            string message = branchAPIResult.Message;
            string branchAPIData = branchAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<List<ActiveFranchiseeContact>>(branchAPIData);
            return branchAPIResult.Code == 200 ? branchData : throw new Exception(message);
        }

        /// <summary>
        /// Create franchisee payment
        /// </summary>
        /// <param name="createPayment"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> CreatePayment(Receipt createPayment)
        {
            dynamic franchiseePaymentAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.PaymentService.CreatePayment, createPayment);
            string message = franchiseePaymentAPIResult.Message;
            string paymentAPIData = franchiseePaymentAPIResult.Data.ToString();
            //var paymentData = JsonConvert.DeserializeObject<string>(paymentAPIData);
            return franchiseePaymentAPIResult.Code == 200 ? paymentAPIData : throw new Exception(message);
        }

        public async Task<string> RollBackPayment(Guid keyValue)
        {
            dynamic paymentAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.PaymentService.RollBackPayment}{keyValue}");
            string message = paymentAPIResult.Message;
            string paymentAPIData = paymentAPIResult.Data.ToString();
            var paymentData = JsonConvert.DeserializeObject<string>(paymentAPIData);
            return paymentAPIResult.Code == 200 ? paymentData! : throw new Exception(message);
        }

        public async Task<string> RollBackPartPayment(Guid tenantBranchTransID)
        {
            dynamic paymentAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.PaymentService.RollBackPartPayment}{tenantBranchTransID}");
            string message = paymentAPIResult.Message;
            string paymentAPIData = paymentAPIResult.Data.ToString();
            var paymentData = JsonConvert.DeserializeObject<string>(paymentAPIData);
            return paymentAPIResult.Code == 200 ? paymentData! : throw new Exception(message);
        }

        /// <summary>
        /// Get tenant branch and contact by id
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<DataSet> GetTenantBranchAndConatctByID(Guid tenantBranchID)
        {
            dynamic branchAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.TenantBranchService.GetBranchAndContactByID}{tenantBranchID}");
            string message = branchAPIResult.Message;
            string branchAPIData = branchAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<DataSet>(branchAPIData);
            return branchAPIResult.Code == 200 ? branchData! : throw new Exception(message);
        }

        public async Task<IEnumerable<CourseBaseInfo>> GetCourseInfoList()
        {
            dynamic courseAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.AcademicService.GetCourseInfoList);
            string message = courseAPIResult.Message;
            string courseAPIData = courseAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<List<CourseBaseInfo>>(courseAPIData);
            return courseAPIResult.Code == 200 ? branchData : throw new Exception(message);
        }

        public async Task<IEnumerable<CourseInfo>> GetCourseBasicInfo()
        {
            dynamic courseAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.AcademicService.GetCourseBasicInfo);
            string message = courseAPIResult.Message;
            string courseAPIData = courseAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<List<CourseInfo>>(courseAPIData);
            return courseAPIResult.Code == 200 ? branchData : throw new Exception(message);
        }

        /// <summary>
        /// Get franchisee code list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="29-04-2023"></param>
        /// <returns>
        /// Returns franchisee codes
        /// </returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<GetFranchiseeCode>> GetFranchiseeID()
        {
            dynamic franchiseeAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.FranchiseeService.GetFranchiseeCodeList);
            string message = franchiseeAPIResult.Message;
            string branchAPIData = franchiseeAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<List<GetFranchiseeCode>>(branchAPIData);
            return franchiseeAPIResult.Code == 200 ? branchData : throw new Exception(message);
        }

        /// <summary>
        /// Get branch contact details
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="02-05-2023"></param>
        /// <returns>
        /// Returns branch contact info
        /// </returns>
        /// <exception cref="Exception"></exception>
        public async Task<GetBranchContactDetail> GetBranchDetails(Guid tenantBranchID)
        {
            dynamic branchAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.TenantBranchService.GetBranchDetails}{tenantBranchID}");
            string message = branchAPIResult.Message;
            string branchAPIData = branchAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<GetBranchContactDetail>(branchAPIData);
            return branchAPIResult.Code == 200 ? branchData : throw new Exception(message);
        }

        /// <summary>
        /// Create student down payment
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> StudentDownPayment(DownPayment payment)
        {
            try
            {
                dynamic studentPaymentAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.PaymentService.StudentDownPayment, payment);
                string message = studentPaymentAPIResult.Message;
                return studentPaymentAPIResult.Code == 201 ? studentPaymentAPIResult.Data : throw new Exception(message);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Create student part payment
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> StudentPartPayment(PartPayment payment)
        {
            dynamic studentPaymentAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.PaymentService.StudentPartPayment, payment);
            string message = studentPaymentAPIResult.Message;
            return studentPaymentAPIResult.Code == 201 ? studentPaymentAPIResult.Data : throw new Exception(message);
        }

        /// <summary>
        /// Get student payment details
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<StudentPaymentDetails>> GetStudentPaymentDetails()
        {
            dynamic studentAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.PaymentService.GetStudentPaymentDetails);
            string message = studentAPIResult.Message;
            string studentAPIData = studentAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<List<StudentPaymentDetails>>(studentAPIData);
            return studentAPIResult.Code == 200 ? branchData : throw new Exception(message);
        }

        /// <summary>
        /// Get student paid fees
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<decimal> GetStudentPaidFees(Guid batchMappingID)
        {
            dynamic studentAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.PaymentService.GetStudentPaidFees}{batchMappingID}");
            string message = studentAPIResult.Message;
            string studentAPIData = studentAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<decimal>(studentAPIData);
            return studentAPIResult.Code == 200 ? branchData : throw new Exception(message);
        }

        /// <summary>
        /// Get material issued list for student
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<MaterialIssuedList>> MaterialIssuedList(string fromDate, string toDate)
        {
            dynamic studentAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.AdmissionService.MaterialIssuedList}{fromDate}&toDate={toDate}");
            string message = studentAPIResult.Message;
            string branchAPIData = studentAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<List<MaterialIssuedList>>(branchAPIData);
            return studentAPIResult.Code == 200 ? branchData : throw new Exception(message);
        }

        /// <summary>
        /// Get material pending list for student
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<MaterialPendingList>> GetMaterialPendingList()
        {
            dynamic studentAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.AdmissionService.GetMaterialPendingList);
            string message = studentAPIResult.Message;
            string branchAPIData = studentAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<List<MaterialPendingList>>(branchAPIData);
            return studentAPIResult.Code == 200 ? branchData : throw new Exception(message);
        }

        /// <summary>
        /// Get material count for specific course
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Int32> GetMaterialCount(Guid batchMappingID)
        {
            dynamic studentAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.AdmissionService.GetMaterialCount}{batchMappingID}");
            string message = studentAPIResult.Message;
            string branchAPIData = studentAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<int>(branchAPIData);
            return studentAPIResult.Code == 200 ? branchData : throw new Exception(message);
        }

        /// <summary>
        /// Get issued material count
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<int> GetIssuedMaterialCount(Guid batchMappingID)
        {
            dynamic studentAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.AdmissionService.GetIssuedMaterialCount}{batchMappingID}");
            string message = studentAPIResult.Message;
            string branchAPIData = studentAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<int>(branchAPIData);
            return studentAPIResult.Code == 200 ? branchData : throw new Exception(message);
        }

        public async Task<int> GetMaterialIssuedCount(Guid materialID)
        {
            dynamic studentAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.AdmissionService.GetMaterialIssuedCount}{materialID}");
            string message = studentAPIResult.Message;
            string branchAPIData = studentAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<int>(branchAPIData);
            return studentAPIResult.Code == 200 ? branchData : throw new Exception(message);
        }

        public async Task<decimal> GetWalletBalance(Guid tenantBranchID)
        {
            dynamic studentAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.PaymentService.GetWalletBalance}{tenantBranchID}");
            string message = studentAPIResult.Message;
            string branchAPIData = studentAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<decimal>(branchAPIData);
            return studentAPIResult.Code == 200 ? branchData : throw new Exception(message);
            //return (decimal)(studentAPIResult.Data);
        }

        public async Task<List<PaymentDetails>> GetPaymentDetails(Guid keyValue)
        {
            dynamic paymentAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.PaymentService.GetPaymentDetails}{keyValue}");
            string message = paymentAPIResult.Message;
            string branchAPIData = paymentAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<List<PaymentDetails>>(branchAPIData);
            return paymentAPIResult.Code == 200 ? branchData : throw new Exception(message);
        }        
        /// <summary>
        /// Get student basic info list
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<StudentBasicInfo>> GetStudentBasicInfo()
        {
            dynamic studentAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.AdmissionService.GetStudentBasicInfo);
            string message = studentAPIResult.Message;
            string studentAPIData = studentAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<List<StudentBasicInfo>>(studentAPIData);
            return studentAPIResult.Code == 200 ? branchData : throw new Exception(message);
        }

        /// <summary>
        /// Get exam indent trans id list
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<GetExamIndentTransID>> ExamIndentTransIDs()
        {
            dynamic indentAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.InventoryService.ExamIndentTransIDs);
            string message = indentAPIResult.Message;
            string indentAPIData = indentAPIResult.Data.ToString();
            var indentData = JsonConvert.DeserializeObject<List<GetExamIndentTransID>>(indentAPIData);
            return indentAPIResult.Code == 200 ? indentData! : throw new Exception(message);
        }

        public async Task<string> UpdateStudentTotalMark(List<StudentTotalMarks> totalMarks)
        {
            dynamic markUpdateAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.InventoryService.UpdateStudentTotalMark, totalMarks);
            string message = markUpdateAPIResult.Message;
            return markUpdateAPIResult.Code == 201 ? message : throw new Exception(message);
        }

        public async Task<DataSet> GetCertificateIssuedList(string fromDate, string toDate)
        {
            dynamic indentAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.AdmissionService.GetCertificateIssuedList}{fromDate}&toDate={toDate}");
            string message = indentAPIResult.Message;
            string indentAPIData = indentAPIResult.Data.ToString();
            var indentData = JsonConvert.DeserializeObject<DataSet>(indentAPIData);
            return indentAPIResult.Code == 200 ? indentData! : throw new Exception(message);
        }

        /// <summary>
        /// Get student course basic info
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<StudentCourseInfo> GetStudentCourseInfo(Guid batchMappingID)
        {
            dynamic studentAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.AdmissionService.GetStudentCourseInfo}{batchMappingID}");
            string message = studentAPIResult.Message;
            string studentAPIData = studentAPIResult.Data.ToString();
            var indentData = JsonConvert.DeserializeObject<StudentCourseInfo>(studentAPIData);
            return studentAPIResult.Code == 200 ? indentData! : throw new Exception(message);
        }

        /// <summary>
        /// Update student edited subject total mark
        /// </summary>
        /// <param name="totalMark"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<int> UpdateStudentEditedMark(GetTotalMark totalMark)
        {
            dynamic updateMarkAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.InventoryService.UpdateStudentEditedMark, totalMark);
            string message = updateMarkAPIResult.Message;
            return updateMarkAPIResult.Code == 201 ? updateMarkAPIResult.Data : throw new Exception(message);
        }

        public async Task<CertificateIndentInfo> GetStudentCertificateIndentInfo(Guid examIndentTransID)
        {
            dynamic indentAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.InventoryService.GetStudentCertificateIndentInfo}{examIndentTransID}");
            string message = indentAPIResult.Message;
            string studentAPIData = indentAPIResult.Data.ToString();
            var indentData = JsonConvert.DeserializeObject<CertificateIndentInfo>(studentAPIData);
            return indentAPIResult.Code == 200 ? indentData! : throw new Exception(message);
        }

        /// <summary>
        /// Get exam indent trans ID by batch mapping ID
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<ExamTransID>> GetExamIndentTransID(Guid batchMappingID)
        {
            dynamic certificateAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.InventoryService.GetExamIndentTransID}{batchMappingID}");
            string message = certificateAPIResult.Message;
            string studentAPIData = certificateAPIResult.Data.ToString();
            var indentData = JsonConvert.DeserializeObject<List<ExamTransID>>(studentAPIData);
            return certificateAPIResult.Code == 200 ? indentData! : throw new Exception(message);
        }

        public async Task<IEnumerable<ContactInfo>> ContactInfo(string contactID)
        {
            dynamic certificateAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.ContactService.ContactInfo}{contactID}");
            string message = certificateAPIResult.Message;
            string studentAPIData = certificateAPIResult.Data.ToString();
            var indentData = JsonConvert.DeserializeObject<IEnumerable<ContactInfo>>(studentAPIData);
            return certificateAPIResult.Code == 200 ? indentData! : throw new Exception(message);
        }

        public async Task<List<Wallet>> GetBranchWalletTrans(Guid tenantBranchID)
        {
            dynamic walletAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.PaymentService.GetBranchWalletTrans}{tenantBranchID}");
            string message = walletAPIResult.Message;
            string studentAPIData = walletAPIResult.Data.ToString();
            var indentData = JsonConvert.DeserializeObject<List<Wallet>>(studentAPIData);
            return walletAPIResult.Code == 200 ? indentData! : throw new Exception(message);
        }

        public async Task<GetTotalPurchase> GetBranchTotalPurchase(Guid tenantBranchID, string fromDate, string toDate)
        {
            dynamic walletAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.InventoryService.GetBranchTotalPurchase}{tenantBranchID}&fromDate={fromDate}&toDate={toDate}");
            string message = walletAPIResult.Message;
            string studentAPIData = walletAPIResult.Data.ToString();
            var indentData = JsonConvert.DeserializeObject<GetTotalPurchase>(studentAPIData);
            return walletAPIResult.Code == 200 ? indentData! : throw new Exception(message);
        }

        public async Task<List<MaterialRequiredList>> MaterialRequiredList(Guid tenantBranchID)
        {
            dynamic materialAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.AdmissionService.MaterialRequiredList}{tenantBranchID}");
            string message = materialAPIResult.Message;
            string studentAPIData = materialAPIResult.Data.ToString();
            var indentData = JsonConvert.DeserializeObject<List<MaterialRequiredList>>(studentAPIData);
            return materialAPIResult.Code == 200 ? indentData! : throw new Exception(message);
        }

        public async Task<StudentGrade> GetStudentGrade(Guid examIndentTransID)
        {
            dynamic gradeAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.InventoryService.GetStudentGrade}{examIndentTransID}");
            string message = gradeAPIResult.Message;
            string studentAPIData = gradeAPIResult.Data.ToString();
            var gradeData = JsonConvert.DeserializeObject<StudentGrade>(studentAPIData);
            return gradeAPIResult.Code == 200 ? gradeData! : throw new Exception(message);
        }

        /// <summary>
        /// Get total semester of course
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<int> GetCourseSemesterCount(Guid batchMappingID)
        {
            dynamic courseAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.AcademicService.GetCourseSemesterCount}{batchMappingID}");
            string message = courseAPIResult.Message;
            string studentAPIData = courseAPIResult.Data.ToString();
            var courseData = JsonConvert.DeserializeObject<int>(studentAPIData);
            return courseAPIResult.Code == 200 ? courseData! : throw new Exception(message);
        }

        public async Task<int> GetStudentSemesterCount(Guid batchMappingID)
        {
            dynamic studentAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.InventoryService.GetStudentSemesterCount}{batchMappingID}");
            string message = studentAPIResult.Message;
            string studentAPIData = studentAPIResult.Data.ToString();
            var studentData = JsonConvert.DeserializeObject<int>(studentAPIData);
            return studentAPIResult.Code == 200 ? studentData! : throw new Exception(message);
        }

        /// <summary>
        /// Get branch wise contact info
        /// </summary>
        /// <param name="contactType"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<GetBranchWiseContactInfo>> GetBranchWiseContactInfo()
        {
            dynamic contactAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.TenantBranchService.GetBranchWiseContactInfo);
            string message = contactAPIResult.Message;
            string contactAPIData = contactAPIResult.Data.ToString();
            var studentData = JsonConvert.DeserializeObject<IEnumerable<GetBranchWiseContactInfo>>(contactAPIData);
            return contactAPIResult.Code == 200 ? studentData! : throw new Exception(message);
        }

        public async Task<int> GetMailCount()
        {
            dynamic emailAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.EmailService.GetMailCount);
            string message = emailAPIResult.Message;
            string emailAPIData = emailAPIResult.Data.ToString();
            var emailData = JsonConvert.DeserializeObject<int>(emailAPIData);
            return emailAPIResult.Code == 200 ? emailData! : throw new Exception(message);
        }

        public async Task<CourseSummaryByAdmin> GetCourseSummaryByAdmin()
        {
            dynamic courseAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.AcademicService.GetCourseSummaryByAdmin);
            string message = courseAPIResult.Message;
            string contactAPIData = courseAPIResult.Data.ToString();
            var courseData = JsonConvert.DeserializeObject<CourseSummaryByAdmin>(contactAPIData);
            return courseAPIResult.Code == 200 ? courseData! : throw new Exception(message);
        }

        public async Task<CourseSummaryByFranchisee> GetCourseSummaryByFranchisee()
        {
            dynamic courseAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.AcademicService.GetCourseSummaryByFranchisee);
            string message = courseAPIResult.Message;
            string contactAPIData = courseAPIResult.Data.ToString();
            var courseData = JsonConvert.DeserializeObject<CourseSummaryByFranchisee>(contactAPIData);
            return courseAPIResult.Code == 200 ? courseData! : throw new Exception(message);
        }

        public async Task<WalletSummary> GetWalletSummary()
        {
            dynamic walletAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.PaymentService.GetWalletSummary);
            string message = walletAPIResult.Message;
            string contactAPIData = walletAPIResult.Data.ToString();
            var walletData = JsonConvert.DeserializeObject<WalletSummary>(contactAPIData);
            return walletAPIResult.Code == 200 ? walletData! : throw new Exception(message);
        }

        public async Task<IndentSummaryByAdmin> GetIndentSummaryByAdmin()
        {
            dynamic indentAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.InventoryService.GetIndentSummaryByAdmin);
            string message = indentAPIResult.Message;
            string contactAPIData = indentAPIResult.Data.ToString();
            var indentData = JsonConvert.DeserializeObject<IndentSummaryByAdmin>(contactAPIData);
            return indentAPIResult.Code == 200 ? indentData! : throw new Exception(message);
        }

        public async Task<IndentSummaryByFranchisee> GetIndentSummaryByFranchisee()
        {
            dynamic indentAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.InventoryService.GetIndentSummaryByFranchisee);
            string message = indentAPIResult.Message;
            string contactAPIData = indentAPIResult.Data.ToString();
            var indentData = JsonConvert.DeserializeObject<IndentSummaryByFranchisee>(contactAPIData);
            return indentAPIResult.Code == 200 ? indentData! : throw new Exception(message);
        }

        public async Task<string> RejectPaidRequest(RejectPayment reject)
        {
            dynamic updateMarkAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.PaymentService.RejectPaidRequest, reject);
            string message = updateMarkAPIResult.Message;
            return updateMarkAPIResult.Code == 201 ? updateMarkAPIResult.Data : throw new Exception(message);
        }

        public async Task<List<Payments>> GetFranchiseePayments(Guid keyValue)
        {
            dynamic paymentAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.PaymentService.GetFranchiseePayments}{keyValue}");
            string message = paymentAPIResult.Message;
            string contactAPIData = paymentAPIResult.Data.ToString();
            var paymentData = JsonConvert.DeserializeObject<List<Payments>>(contactAPIData);
            return paymentAPIResult.Code == 200 ? paymentData! : throw new Exception(message);
        }

        public async Task<List<VendorList>> GetVendorList()
        {
            dynamic vendorAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.UserService.GetVendorList);
            string message = vendorAPIResult.Message;
            string contactAPIData = vendorAPIResult.Data.ToString();
            var vendorData = JsonConvert.DeserializeObject<List<VendorList>>(contactAPIData);
            return vendorAPIResult.Code == 200 ? vendorData! : throw new Exception(message);
        }

        public async Task<string> GetAdminContactID()
        {
            dynamic adminAPIResult = await _dawnHttpClient.GetAsync(DawnServiceURL.ContactService.GetAdminContactID);
            string message = adminAPIResult.Message;
            string adminAPIData = adminAPIResult.Data.ToString();
            return adminAPIResult.Code == 200 ? adminAPIData! : throw new Exception(message);
        }

        public async Task<string> LockFranchisee(ActiveDeactive active)
        {
            try
            {
                dynamic contactAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.UserService.LockFranchisee, active);
                string message = contactAPIResult.Message;
                return contactAPIResult.Code == 201 ? contactAPIResult.Data : throw new Exception(message);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<ExamIndentTransInfo>> GetExamIndentTransList()
        {
            dynamic inventoryAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.InventoryService.GetExamIndentTransList}");
            string message = inventoryAPIResult.Message;
            string inventoryAPIData = inventoryAPIResult.Data.ToString();
            var inventoryData = JsonConvert.DeserializeObject<List<ExamIndentTransInfo>>(inventoryAPIData);
            return inventoryAPIResult.Code == 200 ? inventoryData! : throw new Exception(message);
        }

        public async Task<string> UploadDocument(SaveDocument document)
        {
            try
            {
                dynamic contactAPIResult = await _dawnHttpClient.PostFormAsync(DawnServiceURL.DocumentService.UploadDocument, document);
                string message = contactAPIResult.Message;
                return contactAPIResult.Code == 201 ? contactAPIResult.Data : throw new Exception(message);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<DocumentURLDetails>> GetDocument(List<PrimaryKeyIDs> contactID)
        {
            dynamic contactAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.DocumentService.Base64Document, contactID);
            string message = contactAPIResult.Message;
            string contactAPIData = contactAPIResult.Data.ToString();
            var contactData = JsonConvert.DeserializeObject<List<DocumentURLDetails>>(contactAPIData);
            return contactAPIResult.Code == 200 ? contactData : throw new Exception(message);
        }

        public async Task<List<MblCourse>> GetMblCourseList(Guid tenantBranchID)
        {
            dynamic contactAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.AcademicService.GetMblCourseList}{tenantBranchID}");
            string message = contactAPIResult.Message;
            string contactAPIData = contactAPIResult.Data.ToString();
            var contactData = JsonConvert.DeserializeObject<List<MblCourse>>(contactAPIData);
            return contactAPIResult.Code == 200 ? contactData : throw new Exception(message);
        }

        public async Task<List<PurchaseAmount>> GetPurchaseAmount()
        {
            dynamic contactAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.InventoryService.GetPurchaseAmount}");
            string message = contactAPIResult.Message;
            string contactAPIData = contactAPIResult.Data.ToString();
            var contactData = JsonConvert.DeserializeObject<List<PurchaseAmount>>(contactAPIData);
            return contactAPIResult.Code == 200 ? contactData : throw new Exception(message);
        }

        public async Task<FranchiseeRankStatus> GetFranchiseeRankStatus()
        {
            dynamic contactAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.FranchiseeService.GetFranchiseeRankStatus}");
            string message = contactAPIResult.Message;
            string contactAPIData = contactAPIResult.Data.ToString();
            var contactData = JsonConvert.DeserializeObject<FranchiseeRankStatus>(contactAPIData);
            return contactAPIResult.Code == 200 ? contactData : throw new Exception(message);
        }

        public async Task<List<TenantUserList>> GetTenantUserList()
        {
            dynamic contactAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.UserService.GetTenantUserList}");
            string message = contactAPIResult.Message;
            string contactAPIData = contactAPIResult.Data.ToString();
            var contactData = JsonConvert.DeserializeObject<List<TenantUserList>>(contactAPIData);
            return contactAPIResult.Code == 200 ? contactData : throw new Exception(message);
        }

        public async Task<bool> FranchiseeToAdmin(BranchToAdmin branch)
        {
            try
            {
                dynamic journalAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.AccountingService.FranchiseeToAdmin, branch);
                if (journalAPIResult.Code != 200)
                    return false;
                return true;
            }
            catch (Exception e) { return false; }
        }

        public async Task<bool> BranchToBranch(BranchToBranch branch)
        {
            try
            {
                dynamic journalAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.AccountingService.FranchiseeToAdmin, branch);
                if (journalAPIResult.Code != 200)
                    return false;
                return true;
            }
            catch (Exception e) { return false; }
        }

        public async Task<string> AddBank(AddBankAccount bank)
        {
            dynamic bankAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.AccountingService.AddBank, bank);
            string message = bankAPIResult.Message;
            string bankAPIData = bankAPIResult.Data.ToString();
            return bankAPIResult.Code == 201 ? bankAPIData : throw new Exception(message);
        }

        public async Task<string> GetAdminBranchID(Guid tenantID)
        {
            dynamic adminAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.TenantBranchService.GetAdminBranchID}{tenantID}");
            string message = adminAPIResult.Message;
            string adminAPIData = adminAPIResult.Data.ToString();
            return adminAPIResult.Code == 200 ? adminAPIData : throw new Exception(message);
        }

        public async Task<List<Ledger>> GetAdminBankList()
        {
            dynamic ledgerAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.AccountingService.GetAdminBank}");
            string message = ledgerAPIResult.Message;
            string ledgerAPIData = ledgerAPIResult.Data.ToString();
            var contactData = JsonConvert.DeserializeObject<List<Ledger>>(ledgerAPIData);
            return ledgerAPIResult.Code == 200 ? contactData : throw new Exception(message);
        }

        public async Task<List<Ledger>> GetBranchBankList(Guid branchID)
        {
            dynamic ledgerAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.AccountingService.GetBranchBank}{branchID}");
            string message = ledgerAPIResult.Message;
            string ledgerAPIData = ledgerAPIResult.Data.ToString();
            var contactData = JsonConvert.DeserializeObject<List<Ledger>>(ledgerAPIData);
            return ledgerAPIResult.Code == 200 ? contactData : throw new Exception(message);
        }

        public async Task<bool> FranchiseeToAdmin(List<FranchiseeToAdminPay> payments, Guid giverBranchID, Guid adminBranchID, Guid giverContactID, Guid recieverContactID, Guid paymentID, string narration)
        {
            bool journalPosting = false;
            foreach (var payment in payments)
            {
                NewPaymentDetails details = new NewPaymentDetails();
                details.PaymentModeID = payment.PaymentModeID;
                details.PayModeType = payment.PayModeType;
                details.PayModeValue = payment.Amount;
                details.Amount = payment.Amount;
                details.FromLedgerID = payment.FromLedgerID;
                details.FromLedgerName = payment.FromLedgerName;
                details.ToLedgerID = payment.ToLedgerID;
                details.ToLedgerName = payment.ToLedgerName;
                details.TransactionDate = payment.TransactionDate.ToString();
                details.TransactionNo = payment.TransactionNo;
                var paymentDetail = JsonConvert.SerializeObject(details);
                BranchToAdmin journal = new BranchToAdmin();
                journal.GiverBranchID = giverBranchID;
                journal.ReceiverBranchID = adminBranchID;
                journal.BaseAmount = payment.Amount;
                journal.Narration = narration;
                journal.GiverContactID = giverContactID;
                journal.ReceiverContactID = recieverContactID;
                journal.TransactionAmount = payment.Amount;
                journal.PaymentID = paymentID;
                journal.PaymentDetail = paymentDetail;
                journal.FromLedgerID = payment.FromLedgerID;
                journal.ToLedgerID = payment.ToLedgerID;
                journalPosting = await FranchiseeToAdmin(journal);
            }
            return journalPosting;
        }

        public async Task<bool> BranchToBranchPayment(List<PaymentItemsNew> payments, Guid giverBranchID, Guid giverContactID, Guid recieverContactID, Guid paymentID, string narration)
        {
            bool journalPosting = false;
            foreach (var payment in payments)
            {
                NewPaymentDetails details = new NewPaymentDetails();
                details.PaymentModeID = payment.PaymentModeID;
                details.PayModeType = payment.PayModeType;
                details.PayModeValue = payment.Amount;
                details.Amount = payment.Amount;
                details.FromLedgerID = payment.LedgerID;
                details.FromLedgerName = payment.LedgerName;
                details.TransactionDate = payment.TransactionDate.ToString();
                details.TransactionNo = payment.TransactionNo;
                var paymentDetail = JsonConvert.SerializeObject(details);
                BranchToBranch journal = new BranchToBranch();
                journal.BranchID = giverBranchID;
                journal.BaseAmount = payment.Amount;
                journal.Narration = narration;
                journal.GiverContactID = giverContactID;
                journal.ReceiverContactID = recieverContactID;
                journal.TransactionAmount = payment.Amount;
                journal.PaymentID = paymentID;
                journal.PaymentDetail = paymentDetail;
                journalPosting = await BranchToBranch(journal);
            }
            return journalPosting;
        }

        public async Task<string> GetFranchiseeContactID(Guid branchID)
        {
            dynamic adminAPIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.ContactService.GetFranchiseeContactID}{branchID}");
            string message = adminAPIResult.Message;
            string adminAPIData = adminAPIResult.Data.ToString();
            return adminAPIResult.Code == 200 ? adminAPIData! : throw new Exception(message);
        }

        public async Task<bool> FranchiseeToAdminReEntry(List<FranchiseeToAdminPay> payments, Guid giverBranchID, Guid adminBranchID, Guid giverContactID, Guid recieverContactID, Guid paymentID, string narration)
        {
            bool journalPosting = false;
            foreach (var payment in payments)
            {
                NewPaymentDetails details = new NewPaymentDetails();
                details.PaymentModeID = payment.PaymentModeID;
                details.PayModeType = payment.PayModeType;
                details.PayModeValue = payment.Amount;
                details.Amount = payment.Amount;
                details.FromLedgerID = payment.ToLedgerID;
                details.FromLedgerName = payment.ToLedgerName;
                details.ToLedgerID = payment.FromLedgerID;
                details.ToLedgerName = payment.FromLedgerName;
                details.TransactionDate = payment.TransactionDate.ToString();
                details.TransactionNo = payment.TransactionNo;
                var paymentDetail = JsonConvert.SerializeObject(details);
                BranchToAdmin journal = new BranchToAdmin();
                journal.GiverBranchID = giverBranchID;
                journal.ReceiverBranchID = adminBranchID;
                journal.BaseAmount = payment.Amount;
                journal.Narration = narration;
                journal.GiverContactID = giverContactID;
                journal.ReceiverContactID = recieverContactID;
                journal.TransactionAmount = payment.Amount;
                journal.PaymentID = paymentID;
                journal.PaymentDetail = paymentDetail;
                journal.FromLedgerID = payment.ToLedgerID;
                journal.ToLedgerID = payment.FromLedgerID;
                journalPosting = await FranchiseeToAdmin(journal);
            }
            return journalPosting;
        }

        public async Task<GetPaymentID> GetPaymentID(Guid keyValue)
        {
            dynamic APIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.PaymentService.GetPaymentID}{keyValue}");
            string message = APIResult.Message;
            string APIData = APIResult.Data.ToString();
            var data = JsonConvert.DeserializeObject<GetPaymentID>(APIData);
            return APIResult.Code == 200 ? data : throw new Exception(message);
        }

        public async Task<List<FranchiseeToAdminPay>> GetPaymentItems(Guid keyValue)
        {
            dynamic APIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.PaymentService.GetPaymentItems}{keyValue}");
            string message = APIResult.Message;
            string APIData = APIResult.Data.ToString();
            var data = JsonConvert.DeserializeObject<List<FranchiseeToAdminPay>>(APIData);
            return APIResult.Code == 200 ? data : throw new Exception(message);

        }

        public async Task<List<PurchaseAmount>> GetAdmissionFeesList()
        {
            dynamic APIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.PaymentService.GetAdmissionFeesList}");
            string message = APIResult.Message;
            string APIData = APIResult.Data.ToString();
            var data = JsonConvert.DeserializeObject<List<PurchaseAmount>>(APIData);
            return APIResult.Code == 200 ? data : throw new Exception(message);
        }

        public async Task<bool> WalletToWalletEntry(NewPaymentDetails details, Guid giverBranchID, Guid adminBranchID, Guid giverContactID, Guid recieverContactID, Guid paymentID, string narration)
        {
            bool journalPosting = false;
            var paymentDetail = JsonConvert.SerializeObject(details);
            BranchToAdmin journal = new BranchToAdmin();
            journal.GiverBranchID = giverBranchID;
            journal.ReceiverBranchID = adminBranchID;
            journal.BaseAmount = details.Amount;
            journal.Narration = narration;
            journal.GiverContactID = giverContactID;
            journal.ReceiverContactID = recieverContactID;
            journal.TransactionAmount = details.Amount;
            journal.PaymentID = paymentID;
            journal.PaymentDetail = paymentDetail;
            journal.FromLedgerID = details.ToLedgerID;
            journal.ToLedgerID = details.FromLedgerID;
            journalPosting = await FranchiseeToAdmin(journal);
            return journalPosting;
        }

        public async Task<int> CreateRenewalPayment(WalletRenewalPayment payment)
        {
            dynamic studentAPIResult = await _dawnHttpClient.PostAsync(DawnServiceURL.PaymentService.WalletRenewalPayment, payment);
            string message = studentAPIResult.Message;
            string branchAPIData = studentAPIResult.Data.ToString();
            var branchData = JsonConvert.DeserializeObject<int>(branchAPIData);
            return studentAPIResult.Code == 201 ? branchData : throw new Exception(message);
        }

        public async Task<List<LocalStockList>> GetLocalStockList()
        {
            try
            {
                dynamic APIResult = await _dawnHttpClient.GetAsync($"{DawnServiceURL.InventoryService.GetLocalStockList}");
                if (Convert.ToInt16(APIResult.Code) != 200)
                    throw new Exception("Couldn't get stock !!!");
                string message = APIResult.Message;
                string APIData = APIResult.Data.ToString();
                var data = JsonConvert.DeserializeObject<List<LocalStockList>>(APIData);
                return APIResult.Code == 200 ? data : throw new Exception(message);
            }
            catch (DawnUserException ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }

        public async Task<DocumentResponce> DocumentUpload(ImageDocument document)
        {
            try
            {
                dynamic apiResult = await _dawnHttpClient.PostFormAsync($"{DocumentService.InterimDocument}", document);
                if (Convert.ToInt16(apiResult.Code) != 201)
                    throw new DawnUserException("Couldn't upload the image !");
                string resultData = apiResult.Data.ToString();
                var responseData = JsonConvert.DeserializeObject<DocumentResponce>(resultData) ?? throw new ArgumentNullException(resultData);
                return responseData;
            }
            catch (DawnUserException ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }

        public async Task<string> TempDocumentActivated(TempDocumentActivate activate)
        {
            try
            {
                dynamic APIResult = await _dawnHttpClient.PostAsync(DocumentService.DocumentActivate, activate);
                if (Convert.ToInt16(APIResult.Code) != 201)
                    throw new Exception("Document activation failed.");
                string message = APIResult.Message;
                return APIResult.Code == 201 ? message : throw new Exception(message);
            }
            catch (DawnUserException ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }

        public async Task<string> GetDocumentID(Guid contactID, string documentType)
        {
            try
            {
                dynamic apiResult = await _dawnHttpClient.GetAsync($"{DocumentService.GetDocumentID}{contactID}&DocumentType={documentType}");
                if (Convert.ToInt16(apiResult.Code) != 200)
                    throw new DawnUserException("Could't find document !");
                string message = apiResult.Message;
                string data = apiResult.Data;
                return apiResult.Code == 200 ? data : throw new Exception(message);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }
        public async Task<Guid> OTPVerify(VerifyOTP otpVerify)
        {
            try
            {
                //calling the sms service OTP verification API 
                dynamic smsAPIResult = await _dawnHttpClient.PostAsync($"{SMSServiceURL.VerifyOTP}", otpVerify);
                if (Convert.ToInt16(smsAPIResult.Code) != 200)
                    //throw new Exception("Invalid OTP code !!!");
                    return Guid.Empty;
                string otpresultData = smsAPIResult.Data.ToString();
                return smsAPIResult.Code == 200 ? Guid.Parse(otpresultData) : Guid.Empty;
            }
            catch (DawnUserException ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }

        public async Task<bool> IsOTPChecked(Guid otpTransID)
        {
            try
            {
                //calling the sms service OTP verification API 
                dynamic smsAPIResult = await _dawnHttpClient.GetAsync($"{SMSServiceURL.IsVerifiedCheck}/{otpTransID}");
                string otpresultData = smsAPIResult.Data.ToString();
                return smsAPIResult.Code == 200 ? otpresultData.ToBoolean() : false;
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }

        public async Task<EmailOTPSentResult> SendEmailOTPRequest(EmailOTPRequest otpRequest)
        {
            try
            {
                EmailOTPSentResult otpResponse = new EmailOTPSentResult();
                //calling the sms service  Generate OTP API 
                dynamic smsAPIResult = await _dawnHttpClient.PostAsync($"{EmailServiceURL.OTPGenerate}", otpRequest);
                if (Convert.ToInt16(smsAPIResult.Code) != 200)
                    throw new DawnUserException("Email OTP generation failed !!!");
                string otpresultData = smsAPIResult.Data.ToString();
                var otpResponseData = JsonConvert.DeserializeObject<EmailOTPSentResult>(otpresultData) ?? throw new ArgumentNullException(otpresultData);
                return otpResponseData;
            }
            catch (DawnUserException ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }

        public async Task<bool> IsEmailExistCheck(string emailAddress)
        {
            try
            {
                bool resultData;
                //calling the user service to check email  
                IsEmailCheck emailCheck = new IsEmailCheck()
                {
                    Email = emailAddress
                };
                dynamic apiResult = await _dawnHttpClient.PostAsync($"{UserService.IsEmailCheck}", emailCheck);
                if (Convert.ToInt16(apiResult.Code) != 200)
                    throw new DawnUserException("Verification of email existence failed!");
                bool.TryParse(apiResult.Data.ToString(), out resultData);
                return resultData;
            }
            catch (DawnUserException ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }

        public async Task<bool> IsOTPVerified(Guid otpTransID)
        {
            try
            {
                //calling the sms service OTP verification API 
                dynamic smsAPIResult = await _dawnHttpClient.GetAsync($"{SMSServiceURL.IsOTPVerified}/{otpTransID}");
                return Convert.ToInt16(smsAPIResult.Code) == 200 ? true : false;
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }

        public async Task<string> RegCompletedMail(EmailSendRequest emailSend)
        {
            try
            {
                //calling the sms service to generate success mail
                dynamic smsAPIResult = await _dawnHttpClient.PostAsync($"{EmailServiceURL.EmailSend}", emailSend);
                if (Convert.ToInt16(smsAPIResult.Code) != 200)
                    throw new DawnUserException("Couldn't send email.");
                string message = smsAPIResult.Message;
                return smsAPIResult.Code == 200 ? message : throw new Exception(message);
            }
            catch (DawnUserException ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }

        public async Task<string> GetPaymentTransID(Guid tenantBranchTransID, string transNo)
        {
            try
            {
                dynamic APIResult = await _dawnHttpClient.GetAsync($"{PaymentService.GetPaymentTransID}{tenantBranchTransID}&transNo={transNo}");
                if (Convert.ToInt16(APIResult.Code) != 200)
                    throw new Exception("Data not found");
                string message = APIResult.Message;
                string resultData = APIResult.Data.ToString();
                //var data = JsonConvert.DeserializeObject<List<TenantUserList>>(resultData);
                return APIResult.Code == 200 ? resultData : throw new Exception(message);
            }
            catch (DawnUserException ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }
        public async Task<string> UserLoginCreate(UserLoginCreate createUser)
        {
            try
            {
                dynamic apiResult = await _dawnHttpClient.PostAsync($"{UserService.UserCreate}", createUser);
                if (Convert.ToInt16(apiResult.Code) != 201)
                    throw new DawnUserException(apiResult.Message);
                return apiResult.Message;
            }
            catch (DawnUserException ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }
        public async Task<bool> IsMobileExistCheck(string mobileNumber)
        {
            try
            {
                bool resultData;
                //calling the user service to check mobile number 
                IsMobileCheck mobileCheck = new IsMobileCheck() { Mobile = mobileNumber };
                dynamic apiResult = await _dawnHttpClient.PostAsync($"{UserService.IsMobileCheck}", mobileCheck);
                if (Convert.ToInt16(apiResult.Code) != 200)
                    throw new DawnUserException(apiResult.Message);
                bool.TryParse(apiResult.Data.ToString(), out resultData);
                return resultData;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<OTPResponse> SendSMSOTPRequest(OTPRequest otpRequest)
        {
            try
            {
                OTPResponse otpResponse = new OTPResponse();
                //calling the sms service  Generate OTP API 
                dynamic smsAPIResult = await _dawnHttpClient.PostAsync($"{SMSServiceURL.OTPGenerate}", otpRequest);
                if (Convert.ToInt16(smsAPIResult.Code) != 200)
                    throw new DawnUserException("OTP generation failed !!!");
                string otpresultData = smsAPIResult.Data.ToString();
                var otpResponseData = JsonConvert.DeserializeObject<OTPResponse>(otpresultData) ?? throw new ArgumentNullException(otpresultData);
                return otpResponseData;
            }
            catch (DawnUserException ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }
        public async Task<bool> NotifySMS(SMSQueue sms)
        {
            try
            {
                //MessageInfo messageInfo = new MessageInfo();
                dynamic apiResult = await _dawnHttpClient.PostAsync($"{SMSServiceURL.StudentPasswordSMS}", sms);
                if (Convert.ToInt16(apiResult.Code) != 200)
                    throw new DawnUserException(apiResult.Message);
                return Convert.ToInt16(apiResult.Code) == 200 ? true : false;
            }
            catch (DawnUserException ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }

    }
}