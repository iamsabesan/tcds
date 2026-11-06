using CommonServices;
using Consul.Filtering;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using Dawn.Utilities.Logger;
using EntitiesDTO.Admin;
using EntitiesDTO.Course;
using EntitiesDTO.Material;
using EntitiesDTO.OTP;
using EntitiesDTO.Payment;
using EntitiesDTO.Student;
using EntitiesDTO.Users;
using EntitiesDTO.Utilities;
using Frameworks.Entities;
using Frameworks.Helper;
using iCloud.Admission.Repository;
using MassTransit;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using Utilities.Extensions;

namespace iCloud.Admission.Services {
    public class StudentService {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IdentityInfo _userIdentityInfo;
        private string _connectionString;
        private readonly StudentRepository _studentRepository;
        private readonly CommonService _commonService;
        private readonly IDawnHttpClient _dawnHttpClient;
        #endregion
        #region Constructor
        public StudentService (SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient) {
            _sqlI18N = sqlI18N;
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _connectionString = accessor.GetConnectionString(dawnHttpClient);
            //_connectionString = "Data Source=3.108.251.251;Initial Catalog= iCloud_Academic_98392E8C_A6FC_48D6_873D_5A6C7C514B94_TCDS;Persist Security Info=True;User ID=Identity_db ;Password=Ljgl5871$;TrustServerCertificate=Yes;";
            _commonService = new CommonService(dawnHttpClient);
            _studentRepository = new StudentRepository(_sqlI18N, _connectionString);
            _dawnHttpClient = dawnHttpClient;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Get qualification list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="03-05-2023"></param>
        /// <returns>
        /// Returns qualification list
        /// </returns>
        public List<GetQualification> GetQualificationList () {
            return _studentRepository.GetQualificationList();
        }

        /// <summary>
        /// Get occupation list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="03-05-2023"></param>
        /// <returns>
        /// Returns occupation list
        /// </returns>
        public List<GetOccupation> GetOccupationList () {
            return _studentRepository.GetOccupationList();
        }

        /// <summary>
        /// Register new student
        /// </summary>
        /// <param name="student"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="16-05-2023"></param>
        /// <returns>
        /// Returns student id
        /// </returns>
        /// <exception cref="DawnUserException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<Guid> RegisterStudent (RegisterStudent student) {
            string contactID = string.Empty;
            string addressID = string.Empty;
            string userID = string.Empty;
            string franchiseeNo = string.Empty;
            Guid result = Guid.Empty;
            try {
                RandomPassword randomPassword = new RandomPassword();
                TenantContactUserWrite createUser = student.Mapper<TenantContactUserWrite>();
                createUser.ContactName = student.StudentName;
                createUser.TenantBranchID = _userIdentityInfo.TenantBranchID;
                var franchiseeIds = await _commonService.GetFranchiseeID();
                var franchiseeCode = franchiseeIds.Where(x => x.TenantBranchID == _userIdentityInfo.TenantBranchID).FirstOrDefault();
                franchiseeNo = franchiseeCode?.FranchiseeCode ?? "";
                createUser.DateOfBirth = DateTime.Parse(student.DateOfBirth);
                createUser.Password = randomPassword.Next();
                //createUser.Password = "India_123";
                contactID = await _commonService.CreateStudent(createUser);
                //Get UserID by ContactID
                userID = await _commonService.GetUserID(Guid.Parse(contactID));
                /*Get student contact id*/
                if (contactID == null) {
                    throw new DawnUserException("Contact id not found !");
                }
                /*Insert student address details*/
                List<StudentAddress> address = student.Address;
                foreach (var addressItem in address) {
                    StudentAddress studentAddress = new StudentAddress();
                    studentAddress.ContactAddressID = Guid.NewGuid();
                    studentAddress.ContactID = Guid.Parse(contactID);
                    studentAddress.AddressType = addressItem.AddressType;
                    studentAddress.Address1 = addressItem.Address1;
                    studentAddress.Address2 = addressItem.Address2;
                    studentAddress.Area = addressItem.Area;
                    studentAddress.District = addressItem.District;
                    studentAddress.State = addressItem.State;
                    studentAddress.Country = addressItem.Country;
                    studentAddress.Pincode = addressItem.Pincode;
                    addressID = await _commonService.CreateStudentAddress(studentAddress);
                }
                /*Insert student images*/
                foreach (var documentItem in student.StudentImage) {
                    SaveDocument document = new SaveDocument();
                    document.DocumentID = Guid.NewGuid();
                    document.PrimaryKeyID = Guid.Parse(contactID);
                    document.DocumentType = "Profile";
                    document.DocumentFile = documentItem;
                    string documentID = await _commonService.UploadDocument(document);
                    if (documentID == null || documentID == string.Empty) {
                        var rollBackAddress = _commonService.UserRollBack(userID);
                        throw new DawnUserException("Student image not found");
                    }
                }
                CreateStudent createStudent = student.Mapper<CreateStudent>();
                createStudent.TenantBranchID = _userIdentityInfo.TenantBranchID;
                createStudent.ContactID = Guid.Parse(contactID);
                createStudent.GuardianName = student.FatherName;
                result = _studentRepository.RegisterStudent(createStudent, _userIdentityInfo.ContactID, createUser.Password, franchiseeNo);
                //if (result == Guid.Empty) {
                //    var rollBackAddress = _commonService.UserRollBack(userID);
                //    //var rollBackDocument = await _commonService.RollContactDocument(contactID);
                //    throw new DawnUserException("Student id not found");
                //}
                return result;
            }
            catch(Exception ex) {
                var rollBackContact = _commonService.UserRollBack(userID);
                throw new DawnUserException(ex.Message);
            }
        }

            /// <summary>
            /// Create student course mapping
            /// </summary>
            /// <param name="courseMap"></param>
            /// <param createdBy="sakthivel"></param>
            /// <param createdDate="18-05-2023"></param>
            /// <returns>
            /// Returns student info
            /// </returns>
            /// <exception cref="Exception"></exception>
            public async Task<string> CreateStudentCourseMap (NewStudentCourseMap courseMap) 
            {
            if (_studentRepository.CheckStudentIDIsExists(courseMap.StudentID, courseMap.CourseBatchID))
                throw new DawnUserException("Invalid student id !");
            Guid createdBy = _userIdentityInfo.ContactID;
            Guid tenantBranchID = _userIdentityInfo.TenantBranchID;
            Guid contactID = _studentRepository.GetStudentContactID(courseMap.StudentID);
            /*Get StudentName*/
            string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, connectionString);
            var studentName = commonRepository.GetContactName(contactID);

            string franchiseeContactID = await _commonService.GetFranchiseeContactID(_userIdentityInfo.TenantBranchID);
            string userID = await _commonService.GetUserID(contactID);
            decimal walletBalance = await _commonService.GetWalletBalance(_userIdentityInfo.TenantBranchID);
            decimal admissionFees = _studentRepository.GetAdmissionFees(courseMap.CourseBatchID);
            //Check franchisee wallet balance
            if (admissionFees > walletBalance) {
                if (courseMap.StudentType.ToConditionFormat() == "NEW") {
                    var rollBackContact = _commonService.UserRollBack(userID);
                    int rollbackStudent = _studentRepository.RollBackStudent(courseMap.StudentID);
                }
                throw new DawnUserException("Insufficient wallet balance");
            }
            /*Mapping course to student*/
            StudentCourseMap studentCourse = new StudentCourseMap();
            studentCourse.StudentID = courseMap.StudentID;
            studentCourse.CourseBatchID = courseMap.CourseBatchID;
            studentCourse.Discount = courseMap.Discount;
            studentCourse.DownPayment = courseMap.DownPayment;
            studentCourse.CreatedBy = createdBy;
            var studentCourseMap = _studentRepository.CreateStudentCourseMap(studentCourse);
            if (studentCourseMap == null) {
                if (courseMap.StudentType.ToConditionFormat() == "NEW") {
                    var rollBackContact = _commonService.UserRollBack(userID);
                    int rollbackStudent = _studentRepository.RollBackStudent(courseMap.StudentID);
                }
                throw new DawnUserException("Student creation failed!.");
            }
            List<NewPaymentItems> newItems = new List<NewPaymentItems>();
            foreach (var paymentItems in courseMap.PaymentItems) {
                PaymentItemsNew paymentDetails = paymentItems.Mapper<PaymentItemsNew>();
                var paymentDetail = JsonConvert.SerializeObject(paymentDetails);
                NewPaymentItems newPaymentItems = paymentItems.Mapper<NewPaymentItems>();
                newPaymentItems.PaymentModeID = paymentItems.PaymentModeID;
                newPaymentItems.PayModeType = paymentItems.PayModeType;
                newPaymentItems.TransactionDate = paymentItems.TransactionDate;
                newPaymentItems.BankName = paymentItems.LedgerName;
                newPaymentItems.PaymentDetails = paymentDetail;
                newItems.Add(newPaymentItems);
            }
            /*Get PaymentMode by id*/
            string connStrPayment = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");//wallet payment
            CommonRepository common = new CommonRepository(_sqlI18N, connStrPayment);
            var payModeIDs = courseMap.PaymentItems.Select(p => p.PaymentModeID).Distinct();
            var payModeType = payModeIDs.Select(id => common.GetPayModeType(id)).Where(type => !string.IsNullOrWhiteSpace(type)).ToList();

            DownPayment payment = new DownPayment();
            payment.TenantBranchID = tenantBranchID;
            payment.PaymentItems = newItems;
            payment.KeyValue = studentCourseMap.BatchMappingID;
            payment.CreatedBy = createdBy;
            //string transactionName = "DOWN PAYMENT";
            string transactionName = "ADMISSION FEES"; //changed on 08/08/25
            payment.TransactionName = transactionName;
            payment.WalletAmount = studentCourseMap.AdmissionFees;
            payment.SenderID = createdBy;
            var recieverID = await _commonService.GetAdminContactID();
            payment.ReceiverID = Guid.Parse(recieverID);
            //payment.Description = "Admission Fees : " + studentCourseMap.AdmissionFees + ", Course : " + studentCourseMap.CourseName + ", StudentID : " + studentCourseMap.StudentID;
            payment.Description = $"Registration Fees : {studentCourseMap.AdmissionFees} ( {studentName}, {studentCourseMap.StudentID}, {studentCourseMap.ShortName} ) ({string.Join(", ", payModeType)})";
            string downPayment = await _commonService.StudentDownPayment(payment);
            TempDocumentActivate temp = new TempDocumentActivate();
            List<DocumentItem> documents = new List<DocumentItem>();
            foreach (var item in courseMap.PaymentItems) {
                if (item.DocumentID != Guid.Empty) {
                    string transID = await _commonService.GetPaymentTransID(Guid.Parse(downPayment), item.TransactionNo);
                    documents.Add(new DocumentItem { DocumentID = item.DocumentID, EntityID = Guid.Parse(transID) });
                    temp.DocumentItem = documents;
                }
            }
            if (temp.DocumentItem != null) {
                var activateDocument = await _commonService.TempDocumentActivated(temp);
            }
            if (downPayment == string.Empty || downPayment == null) {
                if (courseMap.StudentType.ToConditionFormat() == "EXISTING") {
                    int rollback = _studentRepository.RollBackStudentCourseMap(courseMap.StudentID, studentCourseMap.BatchMappingID);
                }
                else if (courseMap.StudentType.ToConditionFormat() == "NEW") {
                    int rollback = _studentRepository.RollBackNewStudentCourseMap(courseMap.StudentID, studentCourseMap.BatchMappingID);
                    var rollBackContact = _commonService.UserRollBack(userID);
                }
                throw new DawnUserException("Couldn't mapping the course to student");
            }
            bool accounting = await _commonService.BranchToBranchPayment(courseMap.PaymentItems, tenantBranchID, contactID, Guid.Parse(franchiseeContactID), Guid.Parse(downPayment), transactionName);
            if (accounting == false) {
                if (courseMap.StudentType.ToConditionFormat() == "EXISTING") {
                    var rollBackPayment = await _commonService.RollBackPayment(studentCourseMap.BatchMappingID);
                    int rollback = _studentRepository.RollBackStudentCourseMap(courseMap.StudentID, studentCourseMap.BatchMappingID);
                }
                else if (courseMap.StudentType.ToConditionFormat() == "NEW") {
                    var rollBackPayment = await _commonService.RollBackPayment(studentCourseMap.BatchMappingID);
                    int rollback = _studentRepository.RollBackNewStudentCourseMap(courseMap.StudentID, studentCourseMap.BatchMappingID);
                    var rollBackContact = _commonService.UserRollBack(userID);
                }
                throw new DawnUserException("Couldn't process student down payment");
            }
            string giverContactID = await _commonService.GetFranchiseeContactID(_userIdentityInfo.TenantBranchID);
            string receiverContactID = await _commonService.GetAdminContactID();
            Guid giverBranchID = _userIdentityInfo.TenantBranchID;
            string receiverBranchID = await _commonService.GetAdminBranchID(_userIdentityInfo.TenantID);
            string transactionName1 = "ADMISSION FEES";
            NewPaymentDetails details = new NewPaymentDetails();
            var branchBanks = await _commonService.GetBranchBankList(_userIdentityInfo.TenantBranchID);
            var adminBanks = await _commonService.GetAdminBankList();
            var branchBank = branchBanks.Where(x => x.LedgerName.ToConditionFormat() == "WALLETBANK").FirstOrDefault();
            var adminBank = adminBanks.Where(x => x.LedgerName.ToConditionFormat() == "WALLETBANK").FirstOrDefault();
            details.PaymentModeID = Guid.Empty;
            details.PayModeType = "";
            details.PayModeValue = studentCourseMap.AdmissionFees;
            details.Amount = studentCourseMap.AdmissionFees;
            details.FromLedgerID = adminBank.LedgerID;
            details.FromLedgerName = adminBank.LedgerName;
            details.ToLedgerID = branchBank.LedgerID;
            details.ToLedgerName = branchBank.LedgerName;
            details.TransactionDate = DateTime.Now.ToString();
            bool posting = await _commonService.WalletToWalletEntry(details, giverBranchID, Guid.Parse(receiverBranchID), Guid.Parse(giverContactID), Guid.Parse(receiverContactID), Guid.Parse(downPayment), transactionName);
            if (courseMap.StudentEnquiryID != null || courseMap.StudentEnquiryID.ToString() != string.Empty) {
                int result = _studentRepository.UpdateStudentEnquiry(courseMap.StudentID, _userIdentityInfo.ContactID, courseMap.StudentEnquiryID);
            }

            PasswordSMS sms = new PasswordSMS();
            PaymentSMS paymentSMS = new PaymentSMS();
            /*To send a payment SMS for student*/
            string name = studentName;//commonRepository.GetContactName(contactID);
            var phoneNumber = commonRepository.GetMobileNumber(contactID);

            paymentSMS.StudentName = name;
            paymentSMS.CourseShortName = studentCourseMap.ShortName;
            paymentSMS.Amount = studentCourse.DownPayment;
            paymentSMS.PhoneNumber = phoneNumber.Phone;
            //paymentSMS.PaymentMode = newItems.FirstOrDefault().PayModeType;
            paymentSMS.PaymentMode = $"{newItems.FirstOrDefault().PayModeType} as ADMISSION FEES";
            if (courseMap.StudentType.ToConditionFormat() == "NEW")
            {
                //string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
                //CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
                //string email = commonRepository.GetEmail(contactID);
                string password = _studentRepository.GetPassword(courseMap.StudentID);
                //Send welcome response to franchisee after activation
                //string res = await SendSuccessResponse(email, password, name);

                ///*To send a password SMS for student*/
                var franchiseeIds = await _commonService.GetFranchiseeID();
                var franchiseeCode = franchiseeIds.Where(x => x.TenantBranchID == _userIdentityInfo.TenantBranchID).FirstOrDefault();

                sms.StudentName = name;
                //sms.CourseFranchiseeName = $"{studentCourseMap.ShortName} at {franchiseeCode.FranchiseeShortName}";  //_studentRepository.GetCourseName(result.StudentID); //Get Student enquired course name
                sms.CourseFranchiseeName = $"{studentCourseMap.ShortName} at TCDS";  //_studentRepository.GetCourseName(result.StudentID); //Get Student enquired course name
                sms.Password = password;
                sms.PhoneNumber = phoneNumber.Phone;

                
                /*Send SMS*/                
                    try
                    {
                        await StudentPasswordSMS(sms);
                        await StudentPaymentSMS(paymentSMS);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.Write($"Failed to Send StudentPasswordSMS / StudentPaymentSMS for NEW STUDENT  - {DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt fff")} for ContactId : {_userIdentityInfo.ContactID}, TenantID : {_userIdentityInfo.TenantID}, Message : {ex.Message}", "Error");
                    }
            }
            if(courseMap.StudentType.ToConditionFormat() == "EXISTING")
            {
                try
                {
                    await StudentPaymentSMS(paymentSMS);
                }
                catch (Exception ex)
                {
                    AppLogger.Write($"Failed to Send StudentPaymentSMS For EXISTING STUDENT - {DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt fff")} for ContactId : {_userIdentityInfo.ContactID}, TenantID : {_userIdentityInfo.TenantID}, Message : {ex.Message}", "Error");
                }

            }
            return downPayment;
        }
        /// <summary>
        /// Create student part payment
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <param name="partPayment"></param>
        /// <returns></returns>
        public async Task<string> CreateStudentpartPayment (StudentPartPayment partPayment) {
            string payments = string.Empty;
            decimal walletBalance;
            string walletPaymentID = String.Empty;
            string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");//wallet payment
            CommonRepository common = new CommonRepository(_sqlI18N, connectionString);
            /*Check franchisee wallet balance*/ // added 02/07/2025
            var walletItems = partPayment.PaymentItems.Where(p => p?.PayModeType.ToUpperCase() == "WALLET");
            var walletTotalAmount = walletItems.Sum(p => p.Amount);
            if (walletTotalAmount > 0)
            {                
                walletBalance = common.GetWalletBalance(_userIdentityInfo.TenantBranchID);
                if (walletBalance < walletTotalAmount)
                    throw new DawnUserException("Insufficient wallet balance");
            }
            /*Get PaymentMode by id*/
            var payModeIDs = partPayment.PaymentItems.Select(p => p.PaymentModeID).Distinct();
            var payModeType = payModeIDs.Select(id => common.GetPayModeType(id)).Where(type => !string.IsNullOrWhiteSpace(type)).ToList();

            try {
                string franchiseeContactID = await _commonService.GetFranchiseeContactID(_userIdentityInfo.TenantBranchID);
                string receiverContactID = await _commonService.GetAdminContactID(); //added 02/07/2025
                //Guid studentContactID = _studentRepository.StudentContactID(partPayment.BatchMappingID);
                var studentDetail = _studentRepository.GetContactIDCourse(partPayment.BatchMappingID);
                List<NewPaymentItems> newItems = new List<NewPaymentItems>();
                foreach (var item in partPayment.PaymentItems) {
                    PaymentItemsNew paymentDetails = item.Mapper<PaymentItemsNew>();
                    var paymentDetail = JsonConvert.SerializeObject(paymentDetails);
                    NewPaymentItems newPaymentItems = item.Mapper<NewPaymentItems>();
                    newPaymentItems.PaymentModeID = item.PaymentModeID;
                    newPaymentItems.PayModeType = item.PayModeType;
                    newPaymentItems.TransactionDate = item.TransactionDate;
                    newPaymentItems.PaymentDetails = paymentDetail;
                    newItems.Add(newPaymentItems);
                    /*Create wallet payment */
                    if (item?.PayModeType.ToUpperCase() == "WALLET")
                    {
                        WalletPayment wallet = new WalletPayment();
                        Guid createdBy = _userIdentityInfo.ContactID;
                        wallet.TenantBranchID = _userIdentityInfo.TenantBranchID;
                        wallet.CreatedBy = createdBy;
                        wallet.SenderID = createdBy;
                        wallet.ReceiverID = Guid.Parse(receiverContactID);
                        wallet.WalletAmount = walletTotalAmount;
                        //wallet.Description = "Instalment Fees : " + walletTotalAmount + ", StudentID : " + studentDetail.RollNumber + ", CourseName : " + studentDetail.FullName;
                        wallet.Description = $"Instalment Fees : {walletTotalAmount}, StudentID : {studentDetail.RollNumber}, CourseName : {studentDetail.FullName} ({string.Join(", ", payModeType)})";
                        wallet.WalletType = "INSTALMENT";  //Chnaged PART PAYMENT into INSTALMENT on 08/08/25
                        walletPaymentID = common.CreateWalletPayment(wallet).ToString();
                    }
                }
                PartPayment payment = new PartPayment();
                payment.PaymentItems = newItems;
                payment.KeyValue = partPayment.BatchMappingID;
                payments = await _commonService.StudentPartPayment(payment);
                
                TempDocumentActivate temp = new TempDocumentActivate();
                List<DocumentItem> documents = new List<DocumentItem>();
                foreach (var item in partPayment.PaymentItems) {
                    if (item.DocumentID != Guid.Empty) {
                        string transID = await _commonService.GetPaymentTransID(Guid.Parse(payments), item.TransactionNo);
                        documents.Add(new DocumentItem { DocumentID = item.DocumentID, EntityID = Guid.Parse(transID) });
                        temp.DocumentItem = documents;
                    }
                }
                if (temp.DocumentItem != null) {
                    var activateDocument = await _commonService.TempDocumentActivated(temp);
                }
                bool accounting = await _commonService.BranchToBranchPayment(partPayment.PaymentItems, _userIdentityInfo.TenantBranchID, studentDetail.ContactID, Guid.Parse(franchiseeContactID), Guid.Parse(payments), "INSTALMENT FEES");  //Changed PART PAYMENT Into INSTALMENT FEES on 08/08/25
                //if (accounting == false) {
                //    var rollBackPayment = await _commonService.RollBackPartPayment(Guid.Parse(payments));
                //    throw new DawnUserException("Couldn't process student part payment");
                //}                
                return payments.ToString();
            }
            catch(Exception ex) {
                var rollBackPayment = await _commonService.RollBackPartPayment(Guid.Parse(payments));
                if (walletTotalAmount > 0 && !string.IsNullOrEmpty(walletPaymentID))//Added 02/07/2025
                {
                    int rollbackWallet = common.RollbackWallet(Guid.Parse(walletPaymentID));//Added 02/07/2025
                }
                throw new DawnUserException(ex.Message);
            }
        }

        /// <summary>
        /// Get student alumni list
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="courseID"></param>
        /// <param name="courseBatchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<List<GetAlumniStudent>> GetAlumniStudentList (WebStudentFilter filter) {
            List<GetAlumniStudent> studentList = new List<GetAlumniStudent>();
            var students = _studentRepository.GetAlumniStudentList(filter, _userIdentityInfo.TenantBranchID);
            var contact = await _commonService.ContactInfo(string.Empty);
            var alumni = (from s in students
                          join c in contact on s.ContactID equals c.ContactID
                          select new GetAlumniStudent() {
                              StudentID = s.StudentID,
                              BatchMappingID = s.BatchMappingID,
                              RegisteredDate = s.RegisteredDate,
                              RollNumber = s.RollNumber,
                              StudentName = c.ContactName,
                              Course = s.Course,
                              StartDate = s.StartDate,
                              EndDate = s.EndDate,
                              StartTime = s.StartTime,
                              EndTime = s.EndTime,
                              Phone = c.Phone,
                              AlternatePhone = c.AlternatePhone,
                              Email = c.Email,
                              ContactID = s.ContactID,
                              ShortName = s.ShortName,
                              CourseTypeName = s.CourseTypeName
                          }).ToList();
            return alumni;
        }

        /// <summary>
        /// Get student current list
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="courseID"></param>
        /// <param name="courseBatchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<List<GetAlumniStudent>> GetCurrentStudentList (WebStudentFilter filter) {
            List<GetAlumniStudent> studentList = new List<GetAlumniStudent>();
            var students = _studentRepository.GetCurrentStudentList(filter, _userIdentityInfo.TenantBranchID);
            var contact = await _commonService.ContactInfo(string.Empty);
            var alumni = (from s in students
                          join c in contact on s.ContactID equals c.ContactID
                          select new GetAlumniStudent() {
                              StudentID = s.StudentID,
                              BatchMappingID = s.BatchMappingID,
                              RegisteredDate = s.RegisteredDate,
                              RollNumber = s.RollNumber,
                              StudentName = c.ContactName,
                              Course = s.Course,
                              StartDate = s.StartDate,
                              EndDate = s.EndDate,
                              StartTime = s.StartTime,
                              EndTime = s.EndTime,
                              Phone = c.Phone,
                              AlternatePhone = c.AlternatePhone,
                              Email = c.Email,
                              IsActive = s.IsActive,
                              Reason = s.Reason,
                              ContactID = s.ContactID,
                              ShortName = s.ShortName,
                              CourseTypeName = s.CourseTypeName
                          }).ToList();

            return alumni;
        }
        public async Task<List<GetReprintStudent>> GetReprintingStudent()
        {
            //List<GetReprintStudent> studentList = new List<GetReprintStudent>();
            var students = _studentRepository.GetCompletedStudents(_userIdentityInfo.TenantBranchID);

            string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, connectionString);
            var contact = commonRepository.GetStudentContacts(_userIdentityInfo.TenantBranchID);
            var studentList = (from s in students
                                join c in contact on s.ContactID equals c.ContactID
                                select new GetReprintStudent()
                                {
                                    StudentID = s.StudentID,
                                    ContactID = s.ContactID,
                                    RollNumber = s.RollNumber,
                                    StudentName = c.ContactName,
                                    Phone = c.Phone,
                                    //CourseDetails = new List<GetCourseDetails>()
                                    //{
                                    //   new GetCourseDetails
                                    //   {
                                    //       CourseBatchID = s.CourseBatchID,
                                    //       CourseID = s.CourseID,
                                    //       BatchMappingID = s.BatchMappingID,
                                    //       RollNumber = s.RollNumber,
                                    //       CourseName = s.CourseName,
                                    //       ShortName = s.ShortName,
                                    //       CourseType = s.CourseTypeName,
                                    //       TotalSemesters = s.TotalSemesters,
                                    //       IsCompleted = s.IsCompleted,
                                    //       ReprintCharges = s.ReprintCharges
                                    //   }
                                    //}
                                    CourseDetails = Enumerable.Range(1, s.TotalSemesters).Select(sem => new GetCourseDetails
                                    {
                                        CourseBatchID = s.CourseBatchID,
                                        CourseID = s.CourseID,
                                        BatchMappingID = s.BatchMappingID,
                                        RollNumber = s.RollNumber,
                                        CourseName = s.CourseName,
                                        ShortName = s.ShortName,
                                        CourseType = s.CourseTypeName,
                                        TotalSemesters = s.TotalSemesters,
                                        IsCompleted = s.IsCompleted,
                                        ReprintCharges = s.ReprintCharges,
                                        Semester = sem 
                                    }).ToList()
                                }).ToList();

            var studentsList = studentList
                                .GroupBy(x => new { x.StudentID, x.ContactID, x.RollNumber, x.StudentName, x.Phone })
                                .Select(g => new GetReprintStudent()
                                {
                                    StudentID = g.Key.StudentID,
                                    ContactID = g.Key.ContactID,
                                    RollNumber = g.Key.RollNumber,
                                    StudentName = g.Key.StudentName,
                                    Phone = g.Key.Phone,
                                    CourseDetails = g.SelectMany(x => x.CourseDetails).ToList()

                                }).ToList();
            return studentsList;

        }
        //public async Task<List<StudentMarkReprint>> GetStudentMarkForReprint(Guid batchMappingID,int semester)
        public async Task<List<StudentMarkReprintResult>> GetStudentMarkForReprint(Guid batchMappingID,int semester)
        {
            List<StudentMarkReprint> marks = new List<StudentMarkReprint>();
            string connInventory = await GetConnectionString(_userIdentityInfo.TenantID, "Inventory");
            CommonRepository common = new CommonRepository(_sqlI18N, connInventory);
            var examTransID =  common.GetExamIndentTransID(batchMappingID, semester);
            var ids = examTransID.Select(a => new {
                BatchMappingID = a.StudentBatchMapping,
                ExamIndentTransID = a.ExamIndentTransID }).ToList().ToDataTable();
            var studentMarks = _studentRepository.GetStudentMarkForReprint(ids);

            string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, connectionString);
            var contact = commonRepository.GetStudentContacts(_userIdentityInfo.TenantBranchID);

            var result = studentMarks
                                .GroupBy(x => new {x.StudentID,x.RollNumber,x.ContactID,x.ExamIndentTransID,x.BatchMappingID, x.CourseName })
                                .Select(g => new StudentMarkReprintResult
                                {
                                    StudentID = g.Key.StudentID,
                                    RollNumber = g.Key.RollNumber,
                                    ContactID = g.Key.ContactID,
                                    ExamIndentTransID = g.Key.ExamIndentTransID,
                                    BatchMappingID = g.Key.BatchMappingID,
                                    CourseName = g.Key.CourseName,
                                    Semester = semester,
                                    Subjects = g.Select(m => new StudentMarkSubjectDetail
                                    {
                                        SubjectMappingID = m.SubjectMappingID,
                                        StudentMarkID = m.StudentMarkID,
                                        TheoryMark = m.TheoryMark,
                                        PracticalMark = m.PracticalMark,
                                        ObtainedTheoryMarks = m.ObtainedTheoryMarks,
                                        ObtainedPracticalMarks = m.ObtainedPracticalMarks,
                                        SubjectName = m.SubjectName
                                    }).ToList()
                                }).ToList();

            return result;
        }

        /// <summary>
        /// Get student basic info list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<List<StudentBasicInfo>> GetStudentBasicList () {
            List<StudentBasicInfo> studentList = new List<StudentBasicInfo>();
            var students = _studentRepository.GetStudentInfo();
            //var students = _studentRepository.GetStudentInfo(_userIdentityInfo.TenantBranchID);
            var conatct = await _commonService.ContactInfo(string.Empty);
            var studentInfo = (from s in students
                               join c in conatct on s.ContactID equals c.ContactID
                               select new StudentBasicInfo() {
                                   BatchMappingID = s.BatchMappingID,
                                   StudentID = s.StudentID,
                                   ContactID = c.ContactID,
                                   RollNumber = s.RollNumber,
                                   StudentName = c.ContactName,
                                   Course = s.Course,
                                   Phone = c.Phone,
                                   AlternatePhone = c.AlternatePhone,
                                   BatchNumber = s.BatchNumber,
                                   BatchTime = s.BatchTime,
                                   ShortName = s.ShortName,
                                   CourseTypeName = s.CourseTypeName
                               }).ToList();
            return studentInfo;
        }

        /// <summary>
        /// Get student by franchisee wise
        /// </summary>
        /// <returns></returns>
        public async Task<List<StudentBasicInfo>> GetStudentByBranchWise () {
            List<StudentBasicInfo> studentList = new List<StudentBasicInfo>();
            var students = _studentRepository.GetStudentByBranchWise(_userIdentityInfo.TenantBranchID);
            var conatct = await _commonService.ContactInfo(string.Empty);
            var studentInfo = (from s in students
                               join c in conatct on s.ContactID equals c.ContactID
                               orderby c.ContactName ascending
                               select new StudentBasicInfo() {
                                   BatchMappingID = s.BatchMappingID,
                                   StudentID = s.StudentID,
                                   ContactID = c.ContactID,
                                   RollNumber = s.RollNumber,
                                   StudentName = c.ContactName,
                                   Course = s.Course,
                                   Phone = c.Phone,
                                   AlternatePhone = c.AlternatePhone,
                                   BatchNumber = s.BatchNumber,
                                   BatchTime = s.BatchTime,
                               }).ToList();
            return studentInfo;
        }

        /// <summary>
        /// Get student receipt list
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<List<StudentReceiptList>> GetStudentReceiptList (DateTime fromDate, DateTime toDate, string nameOrPhone) {
            List<StudentReceiptList> list = new List<StudentReceiptList>();
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var payments = commonRepository.GetStudentPaymentDetails(_userIdentityInfo.TenantBranchID, fromDate, toDate);
            var students = _studentRepository.GetStudentReceiptInfo(_userIdentityInfo.TenantBranchID);
            var contact = await _commonService.ContactInfo(string.Empty);
            //var payments = await _commonService.GetStudentPaymentDetails();
            var receiptList = (from s in students
                               join c in contact on s.ContactID equals c.ContactID
                               join p in payments on s.BatchMappingID equals p.KeyValue
                               where string.IsNullOrEmpty(nameOrPhone) || c.Phone.ToLower().Contains(nameOrPhone.ToLower()) || c.ContactName.ToLower().Contains(nameOrPhone.ToLower())
                               select new StudentReceiptList() {
                                   TenantBranchTransID = p.TenantBranchTransID,
                                   StudentID = s.StudentID,
                                   ReceiptDate = p.ReceiptDate,
                                   ReceiptNo = p.ReceiptNo,
                                   RollNumber = s.RollNumber,
                                   StudentName = c.ContactName,
                                   Course = s.Course,
                                   PaymentType = p.PaymentType,
                                   Paymode = p.Paymode,
                                   Amount = p.Amount,
                                   ShortName = s.ShortName,
                                   CourseTypeName = s.CourseTypeName                                   
                               //}).Distinct().OrderByDescending(r =>DateTime.Parse(r.ReceiptDate)).ToList();
                               }).Distinct().OrderByDescending(r => {
                                   DateTime dt;
                                   return DateTime.TryParseExact(
                                       r.ReceiptDate,"dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,out dt ) ? dt : DateTime.MinValue;
                               }).ToList();
            return receiptList;
        }

        /// <summary>
        /// Print student receipt
        /// </summary>
        /// <param name="studentID"></param>
        /// <param name="paymentType"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<StudentReceiptList> PrintStudentReceipt (Guid tenantBranchTransID) {
            var students = _studentRepository.PrintStudentReceipt(_userIdentityInfo.TenantBranchID);
            var contact = await _commonService.ContactInfo(string.Empty);
            var payments = await _commonService.GetStudentPaymentDetails();
            var receiptList = (from s in students
                               join c in contact on s.ContactID equals c.ContactID
                               join p in payments on s.BatchMappingID equals p.KeyValue
                               select new StudentReceiptList() {
                                   TenantBranchTransID = p.TenantBranchTransID,
                                   StudentID = s.StudentID,
                                   ReceiptDate = p.ReceiptDate,
                                   ReceiptNo = p.ReceiptNo,
                                   RollNumber = s.RollNumber,
                                   StudentName = c.ContactName,
                                   Course = s.Course,
                                   PaymentType = p.PaymentType,
                                   Paymode = p.Paymode,
                                   Amount = p.Amount,
                                   SumOfRupees = string.IsNullOrEmpty(p.Amount.ToString()) ? "" : p.Amount.ToDouble().ToAmountInRupees(),
                                   ShortName = s.ShortName,
                                   CourseTypeName = s.CourseTypeName
                               }).Distinct().ToList();
            StudentReceiptList printReceipt = receiptList.Where(x => x.TenantBranchTransID == tenantBranchTransID).FirstOrDefault();
            return printReceipt;
        }

        /// <summary>
        /// Get student outstanding balance list
        /// </summary>
        /// <param name="courseBatchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<List<StudentBalanceList>> GetStudentOutStandingBalance (Guid courseBatchID,string nameOrPhone, string fromDate, string toDate, string status) {
            List<StudentBalanceList> balanceLists = new List<StudentBalanceList>();
            var students = _studentRepository.GetStudentBalanceList(courseBatchID, _userIdentityInfo.TenantBranchID,fromDate,toDate);
            //var contact = await _commonService.ContactInfo(string.Empty);
            string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, connectionString);
            var contact = commonRepository.GetStudentContacts(_userIdentityInfo.TenantBranchID);
            
            var batchMappingIds = students.Select(bm => new { bm.BatchMappingID }).ToList().ToDataTable();
            string connStrPayment = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");// payment
            CommonRepository common = new CommonRepository(_sqlI18N, connStrPayment);
            var feesBalanceList = common.GetFeesBalance(batchMappingIds);
            /*To filter if the nameOrPhone has value*/
            if (!string.IsNullOrEmpty(nameOrPhone))
            {
                var matchedContacts = contact.Where(c =>
                                            c.ContactName.Contains(nameOrPhone, StringComparison.OrdinalIgnoreCase) ||
                                            c.Phone.Contains(nameOrPhone, StringComparison.OrdinalIgnoreCase))
                                            .Select(c => c.ContactID)
                                            .ToList();
                students = students.Where(s => matchedContacts.Contains(s.ContactID)).ToList();
            }
            foreach (var student in students) {
                var contactName = contact.Where(x => x.ContactID == student.ContactID).FirstOrDefault();
                StudentBalanceList balanceList = new StudentBalanceList();
                balanceList.StudentID = student.StudentID;
                balanceList.BatchMappingID = student.BatchMappingID;
                balanceList.ContactID = student.ContactID;
                balanceList.RollNumber = student.RollNumber;
                balanceList.StudentName = contactName != null ? contactName.ContactName : string.Empty;
                balanceList.Course = student.Course;
                balanceList.StartTime = student.StartTime;
                balanceList.EndTime = student.EndTime;
                balanceList.TotalFees = student.TotalFees;
                //balanceList.PaidFees = await _commonService.GetStudentPaidFees(student.BatchMappingID);
                balanceList.PaidFees = feesBalanceList.Where(a => a.BatchMappingID.Equals(student.BatchMappingID)).Select(a => a.Amount).FirstOrDefault();
                balanceList.BalanceFees = balanceList.TotalFees - balanceList.PaidFees;
                balanceList.Status = student.Status;
                balanceList.FeesStatus = (balanceList.TotalFees - balanceList.PaidFees) <= 0 ?  "COMPLETED" : "NOTPAID";
                balanceList.CourseTypeName = student.CourseTypeName;
                balanceList.ShortName = student.ShortName;
                balanceList.StartDate = student.StartDate;
                balanceList.EndDate = student.EndDate;
                //if(balanceList.BalanceFees > 0) {
                //    balanceLists.Add(balanceList);
                //}
                /*This below condition for to add in list who has balance or completd fees*/
                bool balanceListAdd = string.IsNullOrEmpty(status) || status.Equals("All", StringComparison.OrdinalIgnoreCase);

                if (!balanceListAdd && status.Equals("FeesBalance", StringComparison.OrdinalIgnoreCase))
                    balanceListAdd = balanceList.BalanceFees > 0;

                if (!balanceListAdd && status.Equals("FeesCompleted", StringComparison.OrdinalIgnoreCase))
                    balanceListAdd = balanceList.BalanceFees <= 0;

                if (balanceListAdd)
                {
                    balanceLists.Add(balanceList);
                }

            }
            return balanceLists;
        }

        /// <summary>
        /// Get student profile by id
        /// </summary>
        /// <param name="studentID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<StudentProfile> GetStudentProfile (Guid contactID) {
            Guid studentID = _studentRepository.GetStudentID(contactID);
            var contact = await _commonService.ActiveFranchiseeContact(Guid.Empty);
            DataSet studentDataSet = _studentRepository.GetStudentProfile(studentID);
            var studentDetail = studentDataSet.Tables[0].DataTablesToItem<StudentDetail>();
            DataSet contactDataSet = await _commonService.GetTenantBranchAndContact();
            var branch = contactDataSet.Tables[0].DataTablesToList<FranchiseeBaseInfo>().Where(x => x.ContactID == studentDetail.ContactID).FirstOrDefault();
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var mobileNo = commonRepository.GetMobileNumber(studentDetail.ContactID);
            StudentProfile student = new StudentProfile();
            student.ContactID = studentDetail.ContactID;
            student.RollNumber = studentDetail.RollNumber;
            student.Gender = branch.Gender;
            student.StudentName = branch.ContactName;
            student.DateOfBirth = branch.DateOfBirth;
            student.PhoneCode = mobileNo.PhoneCode;
            student.Phone = mobileNo.Phone;
            student.AlternatePhoneCode = mobileNo.AlternatePhoneCode;
            student.AlternatePhone = mobileNo.AlternatePhone;
            student.Email = branch.Email;
            student.Qualification = studentDetail.Qualification;
            student.GuardianType = studentDetail.GuardianType;
            student.FatherName = studentDetail.FatherName;
            student.Occupation = studentDetail.Occupation;
            student.ReasonToSelect = string.IsNullOrEmpty(studentDetail.ReasonToSelect) ? "" : studentDetail.ReasonToSelect;
            student.ReferBy =string.IsNullOrEmpty( studentDetail.ReferBy) ? "" : studentDetail.ReferBy;
            student.Purpose = string.IsNullOrEmpty(studentDetail.Purpose) ? "" : studentDetail.Purpose;
            student.Address = contactDataSet.Tables[1].DataTablesToList<Address>().Where(x => x.ContactID == student.ContactID).ToList();
            List<PrimaryKeyIDs> primaryKeyIDs = new List<PrimaryKeyIDs>();
            PrimaryKeyIDs primaryKey = new PrimaryKeyIDs { ContatcID = student.ContactID };
            primaryKeyIDs.Add(primaryKey);
            student.StudentImages = await _commonService.GetDocument(primaryKeyIDs);
            student.Courses = studentDataSet.Tables[1].DataTablesToList<CourseInfos>().Where(x => x.StudentID == studentDetail.StudentID).ToList();

            //added on 06-03-2025
            foreach (var course in student.Courses)
            {
                course.studentCourse = await GetStudentCourseDetail(course.BatchMappingID);
            }
            //added on 06-03-2025

            return student;
        }

        /// <summary>
        /// Get student course details
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<StudentCourse> GetStudentCourseDetail (Guid batchMappingID) {
            StudentCourse studentCourse = new StudentCourse();
            DataSet courseDataSet = _studentRepository.GetStudentCourseDetail(batchMappingID);
            var course = courseDataSet.Tables[1].DataTablesToItem<CourseDetail>();
            List<NewIssueMaterial> material = courseDataSet.Tables[0].DataTablesToList<NewIssueMaterial>();
            List<MaterialNames> materialName = await _commonService.GetMaterialNameList();
            //var payments = await _commonService.GetStudentPaymentDetails();
            var examTransID = await _commonService.GetExamIndentTransID(batchMappingID);
            List<StudentMarksheetAndCertificate> certificates = _studentRepository.GetStudentCertificate();
            List<StudentMarksheetAndCertificate> marksheets = _studentRepository.GetStudentMarksheet();
            var student = await _commonService.GetStudentBasicInfo();
            var info = student.Where(x => x.BatchMappingID == batchMappingID).FirstOrDefault();
            CertificateMarksheet certificate1 = new CertificateMarksheet();
            foreach (var exam in examTransID) {
                var certificate = certificates.Where(x => x.ExamIndentTransID == exam.ExamIndentTransID).FirstOrDefault();
                if (certificate != null) {
                    certificate1.ExamIndentTransID = exam.ExamIndentTransID;
                    certificate1.CertificateAndMarksheetID = certificate.CertificateAndMarksheetID;
                    certificate1.PrintType = certificate.PrintType;
                    certificate1.Course = info.Course;
                    certificate1.Date = exam.ExamDate;
                    certificate1.IssuedDate = !string.IsNullOrEmpty(certificate.IssuedDate) ? certificate.IssuedDate : "-";
                    certificate1.Status = certificate.IssuedDate == "" ? "Issue" : "Issued";
                    certificate1.ShortName = info.ShortName;
                    certificate1.Semester = exam.Semester;
                }
            }
            List<CertificateMarksheet> marksheet1 = new List<CertificateMarksheet>();
            foreach (var exam in examTransID) {
                var marksheet = marksheets.Where(x => x.ExamIndentTransID == exam.ExamIndentTransID).ToList();
                foreach (var mark in marksheet) {
                    CertificateMarksheet certificateMarksheet = new CertificateMarksheet();
                    certificateMarksheet.ExamIndentTransID = exam.ExamIndentTransID;
                    certificateMarksheet.CertificateAndMarksheetID = mark.CertificateAndMarksheetID;
                    certificateMarksheet.Course = info.Course;
                    certificateMarksheet.ShortName = info.ShortName;
                    certificateMarksheet.PrintType = mark.PrintType;
                    certificateMarksheet.Date = exam.ExamDate;
                    //certificateMarksheet.IssuedDate = mark.IssuedDate != string.IsNullOrEmpty ? mark.IssuedDate : "-";
                    certificateMarksheet.IssuedDate = !string.IsNullOrEmpty(mark.IssuedDate) ? mark.IssuedDate : "-";
                    certificateMarksheet.Status = mark.IssuedDate == "" ? "Issue" : "Issued";
                    certificateMarksheet.Semester = exam.Semester;
                    marksheet1.Add(certificateMarksheet);
                }
            }
            List<Materials> materials = new List<Materials>();
            foreach (var item in material)
            {
                var name = materialName.Find(x => x.MaterialID == item.MaterialID);
                Materials material1 = new Materials();
                material1.MaterialID = item.MaterialID;
                material1.MaterialName = name.MaterialName;
                material1.Semester = item.Semester;
                material1.IssuedDate = item.IssuedDate;
                material1.Status = item.IssuedDate == "" ? "Issue" : "Issued";
                material1.TheoryMarks = item.TheoryMarks;
                material1.PracticalMarks = item.PracticalMarks;
                material1.TotalMarks = item.TotalMarks;
                materials.Add(material1);
            }
            CourseDetail courseDetail = course.Mapper<CourseDetail>();
            courseDetail.PaidFees = await _commonService.GetStudentPaidFees(batchMappingID);
            courseDetail.Balance = course.TotalFees - courseDetail.PaidFees;
            studentCourse.Course = courseDetail;
            var batch = courseDataSet.Tables[2].DataTablesToItem<BatchDetail>();
            var facultyList = await _commonService.GetTenantUserList();
            var faculty = facultyList.Where(x => x.ContactID == batch.FacultyID).FirstOrDefault();
            BatchDetail batchDetail = batch.Mapper<BatchDetail>();
            batchDetail.FacultyName = faculty != null ? faculty.ContactName : string.Empty;
            studentCourse.Batch = batchDetail;
            studentCourse.Materials = materials;
            studentCourse.Payments = await StudentPaymentDetails(batchMappingID);
            studentCourse.Certificate = certificate1;
            studentCourse.Marksheet = marksheet1;
            return studentCourse;
        }

        public async Task<List<StudentPaymentDetails>> StudentPaymentDetails(Guid batchMappingID) {
            List<StudentPaymentDetails> paymentDetails = new List<StudentPaymentDetails>();
            decimal courseFees = 0;
            courseFees = _studentRepository.GetStudentCourseFees(batchMappingID);
            decimal progressPaid = 0;
            var payments = await _commonService.GetStudentPaymentDetails();
            var studentPayment = payments.Where(x => x.KeyValue == batchMappingID).ToList();
            foreach (var item in studentPayment) {
                StudentPaymentDetails payment = new StudentPaymentDetails();
                payment.TenantBranchTransID = item.TenantBranchTransID;
                payment.ReceiptDate = item.ReceiptDate;
                payment.ReceiptNo = item.ReceiptNo;
                payment.Amount = item.Amount;
                progressPaid = progressPaid + item.Amount;
                payment.ProgressPaid = progressPaid;
                courseFees = courseFees - item.Amount;
                payment.Balance = courseFees;
                payment.PaymentType = item.PaymentType;
                payment.Paymode = item.Paymode;
                payment.KeyValue = item.KeyValue;
                payment.Summary = item.Summary;
                var document = await _commonService.GetDocumentID(item.TransID, item.Paymode);
                payment.DocumentID = Guid.Parse(document);
                paymentDetails.Add(payment);
            }
            return paymentDetails;
        }

        /// <summary>
        /// Issue material to student
        /// </summary>
        /// <param name="material"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<string> IssueMaterial (IssueMaterial material) {
            var localStock = await _commonService.GetLocalStockList();
            var stock = localStock.Where(x => x.MaterialID == material.MaterialID).FirstOrDefault();
            if (stock == null || stock.CurrentStock < 0)
                throw new DawnUserException("Not enough current stock to issue material.");
            int result = _studentRepository.IssueMaterial(material, _userIdentityInfo.ContactID);

            /*Send SMS*/      
            try
            {
                await MaterialIssuedSMS(material);
            }
            catch (Exception ex)
            {
                AppLogger.Write($"Failed to Send MaterialIssuedSMS  - {DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt fff")} for ContactId : {_userIdentityInfo.ContactID}, TenantID : {_userIdentityInfo.TenantID}, Message : {ex.Message}", "Error");
            }
            return result > 0 ? "Material Issued successfully." : throw new DawnUserException("Issue failed");
        }

        /// <summary>
        /// Get material issued list
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public IEnumerable<MaterialIssuedList> MaterialIssuedList (string fromDate, string toDate) {
            return _studentRepository.GetMaterialIssuedList(fromDate, toDate, _userIdentityInfo.TenantBranchID);
        }

        /// <summary>
        /// Get material pending list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MaterialPendingList>> GetMaterialPendingList () {
            return _studentRepository.GetMaterialPendingList(_userIdentityInfo.TenantBranchID);
        }

        public IEnumerable<MaterialPendingList> GetMaterialIssuePending (MaterialFilter filter) {
            return _studentRepository.GetMaterialIssuePending(_userIdentityInfo.TenantBranchID, filter);
        }

        /// <summary>
        /// Get material count
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public int GetMaterialCount (Guid batchMappingID) {
            return _studentRepository.GetMaterialCount(batchMappingID);
        }

        /// <summary>
        /// Get issued material count
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public int GetIssuedMaterialCount (Guid batchMappingID) {
            return _studentRepository.GetIssuedMaterialCount(batchMappingID);
        }

        public int GetMaterialIssuedCount (Guid materialID) {
            return _studentRepository.GetMaterialIssuedCount(materialID, _userIdentityInfo.TenantBranchID);
        }

        /// <summary>
        /// Get student actual subject mark list
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param name="courseID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<List<GetStudentMark>> GetStudentMark (Guid courseBatchID) {
            List<GetStudentMark> studentMarks = new List<GetStudentMark>();
            //var indent = await _commonService.ExamIndentTransIDs();

            /*This indent is taken for exam indented*/
            string connInventory = await GetConnectionString(_userIdentityInfo.TenantID, "Inventory");
            CommonRepository common = new CommonRepository(_sqlI18N, connInventory);
            var indent = common.ExamIndentTransID(_userIdentityInfo.TenantBranchID);

            foreach (var item in indent) {
                var student = _studentRepository.GetSubjectsList(item.StudentBatchMapping, item.Semester);
                foreach (var students in student) {
                    var mark = _studentRepository.GetStudentMarkInfo(students.StudentID, students.SubjectID, students.MaterialID, courseBatchID);
                    var marks = mark.Where(x => x.StudentID == students.StudentID).FirstOrDefault();
                    var contactInfo = await _commonService.ContactInfo(string.Empty);
                    var contact = contactInfo.Where(x => x.ContactID == marks?.ContactID).FirstOrDefault();
                    if (marks != null) {
                        if (!_studentRepository.CheckStudentMarkUpdated(item.ExamIndentTransID, marks.SubjectMappingID)) {
                            GetStudentMark getMarks = new GetStudentMark();
                            getMarks.StudentID = students.StudentID;
                            getMarks.ExamIndentTransID = item.ExamIndentTransID;
                            getMarks.ContactID = marks.ContactID;
                            getMarks.StudentName = contact!.ContactName;
                            getMarks.SubjectMappingID = marks.SubjectMappingID;

                            getMarks.BatchMappingID = item.StudentBatchMapping;
                            getMarks.SubjectName = marks.SubjectName;
                            getMarks.RollNumber = marks.RollNumber;
                            getMarks.MaterialID = students.MaterialID;
                            getMarks.Semester = marks.Semester;
                            getMarks.TheoryMarks = marks.TheoryMarks;
                            getMarks.PracticalMarks = marks.PracticalMarks;
                            getMarks.Status = "New";
                            studentMarks.Add(getMarks);
                        }
                    }
                }
            }
            return studentMarks;
        }

        /// <summary>
        /// Update student subject marks
        /// </summary>
        /// <param name="marks"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<string> CreateStudentMarkUpdate (List<UpdateStudentMark> marks) {
            if (marks == null)
                throw new DawnUserException("Invalid data");
            List<UpdateNewStudentMark> studentMarks = new List<UpdateNewStudentMark>();
            if (marks.Any(mark => (mark.ObtainedTheoryMarks > mark.TheoryMarks) || (mark.ObtainedPracticalMarks > mark.PracticalMarks))) {
                throw new DawnUserException("Mark cannot be greater than actual mark.");
            }
            foreach (var mark in marks) {
                UpdateNewStudentMark updateStudentMark = new UpdateNewStudentMark();
                updateStudentMark.ExamIndentTransID = mark.ExamIndentTransID;
                updateStudentMark.SubjectMappingID = mark.SubjectMappingID;
                updateStudentMark.BatchMappingID = mark.BatchMappingID;
                updateStudentMark.Semester = mark.Semester;
                updateStudentMark.TheoryMarks = mark.TheoryMarks;
                updateStudentMark.PracticalMarks = mark.PracticalMarks;
                updateStudentMark.ObtainedTheoryMarks = mark.ObtainedTheoryMarks;
                updateStudentMark.ObtainedPracticalMarks = mark.ObtainedPracticalMarks;
                updateStudentMark.Result = (updateStudentMark.ObtainedTheoryMarks + updateStudentMark.ObtainedPracticalMarks) >= _studentRepository.GetPassMark() ? "PASS" : "FAIL";
                studentMarks.Add(updateStudentMark);
            }
            string indent = await CreateCertificateIndent(studentMarks);
            DataTable updateMark = studentMarks.ToDataTable();
            var result = _studentRepository.CreateStudentMarkUpdate(updateMark, _userIdentityInfo.ContactID);
            List<StudentTotalMarks> students = new List<StudentTotalMarks>();
            students.AddRange(result.Select(item => new StudentTotalMarks {
                ExamIndentTransID = item.ExamIndentTransID,
                TotalMark = item.TotalMark,
                ObtainedMark = item.ObtainedMark
            }));
            var updatedMark = await _commonService.UpdateStudentTotalMark(students);
            return "Certificate send for print successfully";
        }
        /*This is used for mark updatation with preview in franchisee panel*/
        public async Task<List<MarkUpdatePreview>> CreateStudentMarkUpdateNew(List<UpdateStudentMark> marks, bool preview)
        {
            if (marks == null)
                throw new DawnUserException("Invalid data");
            List<UpdateNewStudentMark> studentMarks = new List<UpdateNewStudentMark>();
            if (marks.Any(mark => (mark.ObtainedTheoryMarks > mark.TheoryMarks) || (mark.ObtainedPracticalMarks > mark.PracticalMarks)))
            {
                throw new DawnUserException("Mark cannot be greater than actual mark.");
            }
            foreach (var mark in marks)
            {
                UpdateNewStudentMark updateStudentMark = new UpdateNewStudentMark();
                updateStudentMark.ExamIndentTransID = mark.ExamIndentTransID;
                updateStudentMark.SubjectMappingID = mark.SubjectMappingID;
                updateStudentMark.BatchMappingID = mark.BatchMappingID;
                updateStudentMark.Semester = mark.Semester;
                updateStudentMark.TheoryMarks = mark.TheoryMarks;
                updateStudentMark.PracticalMarks = mark.PracticalMarks;
                updateStudentMark.ObtainedTheoryMarks = mark.ObtainedTheoryMarks;
                updateStudentMark.ObtainedPracticalMarks = mark.ObtainedPracticalMarks;
                updateStudentMark.Result = (updateStudentMark.ObtainedTheoryMarks + updateStudentMark.ObtainedPracticalMarks) >= _studentRepository.GetPassMark() ? "PASS" : "FAIL";
                studentMarks.Add(updateStudentMark);
            }
            if (!preview)
            {
            string indent = await CreateCertificateIndent(studentMarks);
            }
            DataTable updateMark = studentMarks.ToDataTable();
            var result = _studentRepository.CreateStudentMarkUpdateNew(updateMark, _userIdentityInfo.ContactID,preview);
            List<StudentTotalMarks> students = new List<StudentTotalMarks>();
            students.AddRange(result.Select(item => new StudentTotalMarks
            {
                ExamIndentTransID = item.ExamIndentTransID,
                TotalMark = item.TotalMark,
                ObtainedMark = item.ObtainedMark
            }));
            string connInventory = await GetConnectionString(_userIdentityInfo.TenantID, "Inventory");
            CommonRepository common = new CommonRepository(_sqlI18N, connInventory);
            var updatedMark = common.UpdateStudentTotalMark(students.ToDataTable(), _userIdentityInfo.ContactID,preview);
            var previewResult = MarkUpdatesResult(marks);
            return previewResult;
        }
        public List<MarkUpdatePreview> MarkUpdatesResult(List<UpdateStudentMark> marks)
        {
            var indent = marks.Select(a => new { a.ExamIndentTransID,a.BatchMappingID,a.Semester }).Distinct().ToList();
            List<MarkUpdatePreview> result = new List<MarkUpdatePreview>();
            foreach (var indentTran in indent)
            {
                var student = _studentRepository.PrintCertificate(indentTran.BatchMappingID);
                MarkUpdatePreview preview = new MarkUpdatePreview();
                preview.ExamIndentTransID = indentTran.ExamIndentTransID;
                preview.BatchMappingID = indentTran.BatchMappingID;
                preview.Course = student.Course;
                preview.StudentName = "";
                preview.RollNumber = student.RollNumber;
                preview.Semester = indentTran.Semester;
                preview.SemesterResult = indentTran.Semester == student.TotalSemesters ? "Final" : indentTran.Semester.ToString();
                result.Add(preview);
            }
            return result;
        }
        public async Task<string> CreateCertificateIndent (List<UpdateNewStudentMark> marks) {
            DataTable data = marks.ToDataTable();
            var indent = _studentRepository.GetExamIndentTransID(data);
            List<CertificateIndent> indentList = new List<CertificateIndent>();
            foreach (var indentTrans in indent) {
                int count = _studentRepository.CheckExamIndentTransIDExists(indentTrans.ExamIndentTransID);
                if (count > 0)
                    throw new DawnUserException("Indent already created");
                int semester = await _commonService.GetCourseSemesterCount(indentTrans.BatchMappingID);
                if (semester == indentTrans.Semester) {
                    CertificateIndent certificate = new CertificateIndent();
                    certificate.ExamIndentTransID = indentTrans.ExamIndentTransID;
                    certificate.PrintType = "Certificate";
                    indentList.Add(certificate);
                    CertificateIndent certificate1 = new CertificateIndent();
                    certificate1.ExamIndentTransID = indentTrans.ExamIndentTransID;
                    certificate1.PrintType = "Marksheet";
                    indentList.Add(certificate1);
                    int result = _studentRepository.UpdateStudentCompleted(indentTrans.BatchMappingID, _userIdentityInfo.ContactID);
                }
                else if (indentTrans.Semester < semester) {
                    CertificateIndent certificate = new CertificateIndent();
                    certificate.ExamIndentTransID = indentTrans.ExamIndentTransID;
                    certificate.PrintType = "Marksheet";
                    indentList.Add(certificate);
                }
            }
            string certificateIndent = CreateCertificateAndMarkSheetIndent(indentList);
            if (certificateIndent == null || certificateIndent == string.Empty) {
                throw new DawnUserException("Couldn't create exam indent");
            }
            return certificateIndent;
        }

        public string CreateCertificateAndMarkSheetIndent (List<CertificateIndent> indents) {
            DataTable certificateIndent = indents.ToDataTable();
            int result = _studentRepository.CreateCertificateIndent(certificateIndent, _userIdentityInfo.ContactID, _userIdentityInfo.TenantBranchID);
            return result > 0 ? "Indent request send successfully !" : throw new DawnUserException("Couldn't send indent request !");
        }
        public async Task<string> CreateCertificateIndentReprinting(List<CertificateIndent> ExamIndentTransIDs)
        {
            DataTable dataList = ExamIndentTransIDs.ToDataTable();
            int result = _studentRepository.CreateCertificateIndent(dataList, _userIdentityInfo.ContactID, _userIdentityInfo.TenantBranchID);
            return result > 0 ? "Indent request send successfully !" : throw new DawnUserException("Couldn't send indent request !");                       
        }
        /// <summary>
        /// Get certificate indent list
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="status"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public DataSet GetCertificateIndentList (string fromDate, string toDate, string status) {
            return _studentRepository.GetCertificateIndent(fromDate, toDate, status, _userIdentityInfo.TenantBranchID);
        }
        public DataSet GetCertificateIssuedList (string fromDate, string toDate) {
            return _studentRepository.GetCertificateIssuedList(fromDate, toDate, _userIdentityInfo.TenantBranchID);
        }
        /// <summary>
        /// Get student course basic info
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public StudentCourseInfo GetStudentCourseInfo (Guid batchMappingID) {
            return _studentRepository.GetStudentCourseInfo(batchMappingID);
        }

        /// <summary>
        /// Edit student subject marks
        /// </summary>
        /// <param name="editMarks"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<string> EditStudentMark (List<EditMark> editMarks) {
            foreach (EditMark editMark in editMarks) {
                if (!_studentRepository.CheckExamIndentTransID(editMark.ExamIndentTransID))
                    throw new Exception("Couldn't edit mark !");
                int count = _studentRepository.CheckMark(editMark.ExamIndentTransID, editMark.SubjectMappingID, editMark.ObtainedTheoryMarks, editMark.ObtainedPracticalMarks);
                if (count > 0)
                    throw new DawnUserException("Mark cannot be greater than actual mark");
            }
            DataTable marks = editMarks.ToDataTable();
            var totalMarks = _studentRepository.EditStudentMark(marks, _userIdentityInfo.ContactID);
            GetTotalMark getTotal = new GetTotalMark();
            getTotal.ExamIndentTransID = totalMarks.ExamIndentTransID;
            getTotal.TotalMarks = totalMarks.TotalMarks;
            int result = await _commonService.UpdateStudentEditedMark(getTotal);
            return result > 0 ? "Mark updated successfully !" : "Couldn't update mark !";
        }

        /// <summary>
        /// Get certificate despatch list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<List<CertificateDespatchList>> GetCertificateDespatchList () {
            List<CertificateDespatchList> certificateDespatchLists = new List<CertificateDespatchList>();
            var certificate = _studentRepository.CertificateDespatchList();
            DataTable branchIdsTable = new DataTable();
            branchIdsTable.Columns.Add("TenantBranchID", typeof(Guid));
            foreach (var item in certificate)
            {
                branchIdsTable.Rows.Add(item.TenantBranchID);
            }
            string franchiseeConn = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepoo = new CommonRepository(_sqlI18N, franchiseeConn);
            var branchDetailsList = commonRepoo.GetBranchDetail(branchIdsTable);

            foreach (var item in certificate) {
                var certificateInfo = await _commonService.GetStudentCertificateIndentInfo(item.ExamIndentTransID);
                var courseInfo = _studentRepository.GetStudentCourseInfo(certificateInfo.StudentBatchMapping);
                //var branch = await _commonService.GetBranchDetails(item.TenantBranchID);
                var branch = branchDetailsList.Where(x => x.TenantBranchID == item.TenantBranchID).FirstOrDefault();
                var franchisee = await _commonService.GetFranchiseeID();
                var franchiseeID = franchisee.Where(x => x.TenantBranchID == item.TenantBranchID).FirstOrDefault();
                
                string connTenant = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
                using CommonRepository commonRepo = new CommonRepository(_sqlI18N, connTenant);
                var studentName = commonRepo.GetContactName(courseInfo.ContactID);
                
                CertificateDespatchList despatchList = new CertificateDespatchList();
                despatchList.TenantBranchID = item.TenantBranchID;
                despatchList.ExamIndentTransID = item.ExamIndentTransID;
                despatchList.IndentNumber = item.IndentNumber;
                despatchList.IndentDate = item.IndentDate;
                despatchList.FranchiseeID = franchiseeID != null ? franchiseeID.FranchiseeCode : "";
                despatchList.FranchiseeName = branch.BranchName;
                despatchList.Address1 = branch.Address1;
                despatchList.Address2 = branch.Address2;
                despatchList.Area = branch.Area;
                despatchList.District = branch.District;
                despatchList.State = branch.State;
                despatchList.Country = branch.Country;
                despatchList.Pincode = branch.Pincode;
                despatchList.StudentID = courseInfo.RollNumber;
                despatchList.StudentName = studentName;
                despatchList.Course = courseInfo.Course;
                despatchList.Semester = certificateInfo.Semester.ToString();
                despatchList.Type = item.Type;
                despatchList.Taluk = branch.Taluk;
                certificateDespatchLists.Add(despatchList);
            }
            return certificateDespatchLists;
        }

        /// <summary>
        /// Despatch certificate and marksheet indent
        /// </summary>
        /// <param name="despatches"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public string DespatchCertificateAndMarkSheet (List<DespatchCertificate> despatches) {
            DataTable certificate = despatches.ToDataTable();
            int count = _studentRepository.DespatchCertificateAndMarkSheet(certificate, _userIdentityInfo.ContactID);
            if (count == 0)
                throw new DawnUserException("Could not despatch !");
            return "Despatch successful !";
        }

        /// <summary>
        /// Receive certificate and marksheet
        /// </summary>
        /// <param name="receive"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public string ReceiveCertificateAndMarksheet (List<ReceiveCertificate> receive) {
            DataTable certificate = receive.ToDataTable();
            int count = _studentRepository.ReceiveCertificateAndMarksheet(certificate, _userIdentityInfo.ContactID);

            return count > 0 ? "Received successfully !" : throw new DawnUserException("Could not receive !");
        }

        /// <summary>
        /// Get certificate print list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<List<NewCertificatePrintList>> GetCertificatePrintList () {
            List<NewCertificatePrintList> certificatePrintLists = new List<NewCertificatePrintList>();
            var indent = _studentRepository.GetCertificateIndentList("Certificate");
            var examTransList = await _commonService.GetExamIndentTransList();

            DataTable branchIdsTable = new DataTable();
            branchIdsTable.Columns.Add("TenantBranchID", typeof(Guid));
            foreach (var item in indent)
            {
                branchIdsTable.Rows.Add(item.TenantBranchID);
            }
            string franchiseeConn = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepo = new CommonRepository(_sqlI18N, franchiseeConn);
            var branchDetailsList = commonRepo.GetBranchDetail(branchIdsTable);

            foreach (var item in indent) {
                var certificate = _studentRepository.GetCertificatePrintList("Certificate", item.CertificateIndentID);
                var indentTransDetail = (from c in certificate
                                         join e in examTransList on c.ExamIndentTransID equals e.ExamIndentTransID
                                         select new ExamIndentTransInfo() {
                                             ExamIndentTransID = c.ExamIndentTransID,
                                             StudentBatchMapping = e.StudentBatchMapping,
                                             ExamDate = e.ExamDate,
                                             Semester = e.Semester,
                                             TotalMark = e.TotalMark,
                                             Grade = e.Grade,
                                         });
                int num = 0;
                int totalItem = item.TotalItem;
                List<StudentTotalSem> semester = new List<StudentTotalSem>();
                foreach (var count in indentTransDetail) {
                    var check = _studentRepository.CheckCertificatePrinted(count.ExamIndentTransID);
                    if (check == true)
                        num++;
                    StudentTotalSem sem = new StudentTotalSem();
                    sem.ExamIndentTransID = count.ExamIndentTransID;
                    sem.Semester = count.Semester + "/" + (await _commonService.GetCourseSemesterCount(count.StudentBatchMapping));
                    semester.Add(sem);
                }
                var studentInfo = _studentRepository.GetStudentCourseInfoList();
                //var branch = await _commonService.GetBranchDetails(item.TenantBranchID);
                var branch = branchDetailsList.Where(x=>x.TenantBranchID == item.TenantBranchID).FirstOrDefault();
                var franchisee = await _commonService.GetFranchiseeID();
                var franchiseeID = franchisee.Where(x => x.TenantBranchID == item.TenantBranchID).FirstOrDefault();
                var contact = await _commonService.ContactInfo(string.Empty);
                var students = (from c in certificate
                                join i in indentTransDetail on c.ExamIndentTransID equals i.ExamIndentTransID
                                join sem in semester on c.ExamIndentTransID equals sem.ExamIndentTransID
                                join s in studentInfo on i.StudentBatchMapping equals s.BatchMappingID
                                join co in contact on s.ContactID equals co.ContactID
                                select new StudentCertificateInfo() {
                                    StudentID = s.StudentID,
                                    ExamIndentTransID = i.ExamIndentTransID,
                                    CertificateAndMarksheetID = c.CertificateAndMarksheetID,
                                    RollNumber = s.RollNumber,
                                    StudentName = co.ContactName,
                                    Course = s.Course,
                                    Semester = sem.Semester,
                                    ExamDate = i.ExamDate,
                                    Batch = s.BatchTime,
                                    Type = c.Type,
                                    ToatlMarks = i.TotalMark,
                                    Grade = i.Grade?.ToUpper(),
                                    Status = c.Status,
                                    CourseTypeName = s.CourseTypeName,
                                    ShortName = s.ShortName
                                }).ToList();
                if (totalItem != 0) {
                    NewCertificatePrintList printList = new NewCertificatePrintList();
                    printList.CertificateIndentID = item.CertificateIndentID;
                    printList.IndentDate = item.IndentDate;
                    printList.IndentNumber = item.IndentNumber;
                    printList.TenantBranchID = item.TenantBranchID;
                    printList.FranchiseeID = franchiseeID != null ? franchiseeID.FranchiseeCode : "";
                    printList.FranchiseeName = branch.BranchName;
                    printList.Address1 = branch.Address1;
                    printList.Address2 = branch.Address2;
                    printList.Area = branch.Area;
                    printList.District = branch.District;
                    printList.State = branch.State;
                    printList.Country = branch.Country;
                    printList.Pincode = branch.Pincode;
                    printList.Taluk = branch.Taluk;
                    printList.Total = totalItem;
                    printList.Printed = num;
                    printList.Pending = totalItem - num;
                    if (totalItem == (totalItem - num)) {
                        printList.Status = "PENDING";
                    }
                    else {
                        printList.Status = (totalItem - num) == 0 ? "PRINTED" : "PENDING";
                    }
                    printList.StudentInfo = students;
                    certificatePrintLists.Add(printList);
                }
            }
            return certificatePrintLists;
        }

        /// <summary>
        /// Get marksheet print list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<List<NewMarkSheetPrintList>> GetMarkSheetPrintList () {
            List<NewMarkSheetPrintList> markSheetPrintLists = new List<NewMarkSheetPrintList>();
            var indent = _studentRepository.GetCertificateIndentList("Marksheet");
            var examTransList = await _commonService.GetExamIndentTransList();

            DataTable branchIdsTable = new DataTable();
            branchIdsTable.Columns.Add("TenantBranchID", typeof(Guid));
            foreach (var item in indent)
            {
                branchIdsTable.Rows.Add(item.TenantBranchID);
            }
            string franchiseeConn = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepo = new CommonRepository(_sqlI18N, franchiseeConn);
            var branchDetailsList = commonRepo.GetBranchDetail(branchIdsTable);

            foreach (var item in indent) {
                var certificate = _studentRepository.GetCertificatePrintList("Marksheet", item.CertificateIndentID);
                var indentTransDetail = (from c in certificate
                                         join e in examTransList on c.ExamIndentTransID equals e.ExamIndentTransID
                                         select new ExamIndentTransInfo() {
                                             ExamIndentTransID = c.ExamIndentTransID,
                                             StudentBatchMapping = e.StudentBatchMapping,
                                             ExamDate = e.ExamDate,
                                             Semester = e.Semester,
                                             TotalMark = e.TotalMark,
                                             Grade = e.Grade,
                                         });
                int num = 0;
                int totalItem = item.TotalItem;
                List<StudentTotalSem> semester = new List<StudentTotalSem>();
                foreach (var count in indentTransDetail) {
                    var check = _studentRepository.CheckCertificatePrinted(count.ExamIndentTransID);
                    if (check == true)
                        num++;
                    StudentTotalSem sem = new StudentTotalSem();
                    sem.ExamIndentTransID = count.ExamIndentTransID;
                    sem.Semester = count.Semester + "/" + (await _commonService.GetCourseSemesterCount(count.StudentBatchMapping));
                    semester.Add(sem);
                }
                var studentInfo = _studentRepository.GetStudentCourseInfoList();
                var marks = _studentRepository.GetStudentObtainedMarks();
                //var branch = await _commonService.GetBranchDetails(item.TenantBranchID);
                var branch = branchDetailsList.Where(x => x.TenantBranchID == item.TenantBranchID).FirstOrDefault();
                var franchisee = await _commonService.GetFranchiseeID();
                var franchiseeID = franchisee.Where(x => x.TenantBranchID == item.TenantBranchID).FirstOrDefault();
                var contact = await _commonService.ContactInfo(string.Empty);
                var students = (from m in certificate
                                join i in indentTransDetail on m.ExamIndentTransID equals i.ExamIndentTransID
                                join sem in semester on m.ExamIndentTransID equals sem.ExamIndentTransID
                                join o in marks on m.ExamIndentTransID equals o.ExamIndentTransID
                                join s in studentInfo on i.StudentBatchMapping equals s.BatchMappingID
                                join co in contact on s.ContactID equals co.ContactID
                                select new StudentMarkSheetInfo() {
                                    StudentID = s.StudentID,
                                    ExamIndentTransID = i.ExamIndentTransID,
                                    CertificateAndMarksheetID = m.CertificateAndMarksheetID,
                                    RollNumber = s.RollNumber,
                                    StudentName = co.ContactName,
                                    Course = s.Course,
                                    Semester = sem.Semester,
                                    ExamDate = i.ExamDate,
                                    Batch = s.BatchTime,
                                    Type = m.Type,
                                    TheoryMarks = o.TheoryMarks,
                                    PracticalMarks = o.PracticalMarks,
                                    Grade = i.Grade,
                                    Status = m.Status,
                                    ShortName = s.ShortName
                                }).ToList();
                if (totalItem != 0) {
                    NewMarkSheetPrintList printList = new NewMarkSheetPrintList();
                    printList.CertificateIndentID = item.CertificateIndentID;
                    printList.IndentDate = item.IndentDate;
                    printList.IndentNumber = item.IndentNumber;
                    printList.TenantBranchID = item.TenantBranchID;
                    printList.FranchiseeID = franchiseeID != null ? franchiseeID.FranchiseeCode : "";
                    printList.FranchiseeName = branch.BranchName;
                    printList.Address1 = branch.Address1;
                    printList.Address2 = branch.Address2;
                    printList.Area = branch.Area;
                    printList.District = branch.District;
                    printList.State = branch.State;
                    printList.Country = branch.Country;
                    printList.Pincode = branch.Pincode;
                    printList.Taluk = branch.Taluk;
                    printList.Total = totalItem;
                    printList.Printed = num;
                    printList.Pending = totalItem - num;
                    if (totalItem == 0) {
                        printList.Status = "PENDING";
                    }
                    else {
                        printList.Status = (totalItem - num) == 0 ? "PRINTED" : "PENDING";
                    }
                    printList.StudentInfo = students;
                    markSheetPrintLists.Add(printList);
                }
            }
            return markSheetPrintLists;
        }

        /// <summary>
        /// Issue certificate and marksheet to student
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param name="printType"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<string> IssueCertificateAndMarksheet (Guid certificateAndMarksheetID) {
            int result = _studentRepository.IssueCertificateAndMarksheet(certificateAndMarksheetID, _userIdentityInfo.ContactID);
            if (result == 0)
                throw new DawnUserException("Couldn't issue");
            /*Send SMS*/                       
                try
                {
                    await CertificateMarksheetSMS(certificateAndMarksheetID);
                }
                catch (Exception ex)
                {
                    AppLogger.Write($"Failed to Send CertificateMarksheetSMS  - {DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt fff")} for ContactId : {_userIdentityInfo.ContactID}, TenantID : {_userIdentityInfo.TenantID}, Message : {ex.Message}", "Error");
                }
            return "Issued successfully";
        }

        /// <summary>
        /// Get material required list
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public List<MaterialRequiredList> MaterialRequiredList (Guid tenantBranchID) {
            List<MaterialRequiredList> materialRequiredLists = new List<MaterialRequiredList>();
            DataSet materialDataSet = _studentRepository.MaterialRequiredList(tenantBranchID);
            var materialIssue = materialDataSet.Tables[0].DataTablesToList<MaterialRequiredList>();
            var materialIssued = materialDataSet.Tables[1].DataTablesToList<MaterialRequiredList>();
            foreach (var material in materialIssue) {
                var issued = materialIssued.Where(x => x.MaterialID == material.MaterialID).FirstOrDefault();
                MaterialRequiredList materialRequired = new MaterialRequiredList();
                int count = issued != null ? issued.Total : 0;
                materialRequired.MaterialID = material.MaterialID;
                materialRequired.Total = material.Total - count;
                materialRequiredLists.Add(materialRequired);
            }
            return materialRequiredLists;
        }

        /// <summary>
        /// Print certificate
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<CertificatePrint> CreateCertificatePrint (Guid examIndentTransID) {
            var grade = await _commonService.GetStudentGrade(examIndentTransID);
            var student = _studentRepository.PrintCertificate(grade.BatchMappingID);
            var contactInfo = await _commonService.ContactInfo(string.Empty);
            //var branch = await _commonService.GetBranchDetails(student.TenantBranchID);
            /*Get tenant branch details*/
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var branch = commonRepository.GetBranchDetails(student.TenantBranchID);
            var name = contactInfo.Where(x => x.ContactID == student.ContactID).FirstOrDefault();
            var image = await _commonService.GetDocument(new List<PrimaryKeyIDs> {
                new PrimaryKeyIDs { ContatcID = student.ContactID }
                });
            var doc = image.FirstOrDefault();
            CertificatePrint certificatePrint = new CertificatePrint();
            certificatePrint.BatchMappingID = grade.BatchMappingID;
            certificatePrint.StudentName = (name?.ContactName ?? "").ToUpper();
            certificatePrint.RollNumber = student.RollNumber;
            certificatePrint.ShortName = student.ShortName;
            certificatePrint.Course = student.Course;
            certificatePrint.StartDate = student.StartDate;
            certificatePrint.EndDate = student.EndDate;
            certificatePrint.BranchName = branch.BranchName;
            //certificatePrint.Address = branch.Address1 + "," + branch.Address2 + "," + branch.Area + "," + branch.District + "," + branch.State + "," + branch.Country + "," + branch.Pincode;
            certificatePrint.Address = branch.Area + (string.IsNullOrWhiteSpace(branch.Taluk) ? "" : ", " + branch.Taluk) + ", " + branch.District + ", " + branch.Pincode;
            certificatePrint.Grade = grade.GradeName;
            certificatePrint.DateOfIssue = grade.DateOfIssue;
            certificatePrint.ProfileBase64 = doc?.ProfileBase64 ?? string.Empty;
            certificatePrint.MIMEType = doc?.MIMEType ?? string.Empty;
            certificatePrint.CourseTypeName = student.CourseTypeName;
            certificatePrint.Subjects = _studentRepository.GetSubjectsName(grade.BatchMappingID);
            return certificatePrint;
        }
        public async Task<List<CertificatePrint>> PreviewCertificate(List<PreviewCretificateMarksheet> transID)
        {
            var certificate = new List<CertificatePrint>();
            foreach (var examIndent in transID)
            {
                var printCertificate = await CreateCertificatePrint(examIndent.ExamIndentTransID);
                printCertificate.DateOfIssue = DateTime.Now.ToString("dd-MM-yyyy");
                certificate.Add(printCertificate);
            }
            return certificate;
        }
        public async Task<List<MarksheetPrint>> PreviewMarksheet(List<PreviewCretificateMarksheet> transID)
        {
            var marksheets = new List<MarksheetPrint>();
            foreach(var examIndent in transID)
            {
                var printMarksheet = await PrintMarksheet(examIndent.ExamIndentTransID);
                printMarksheet.DateOfIssue = DateTime.Now.ToString("dd-MM-yyyy");
                marksheets.Add(printMarksheet);
            }
            return marksheets;
        }
        /// <summary>
        /// Print marksheet
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <returns></returns>
        public async Task<MarksheetPrint> PrintMarksheet (Guid examIndentTransID) {
            DataSet subjectDataSet = _studentRepository.PrintMarksheet(examIndentTransID);
            var grade = await _commonService.GetStudentGrade(examIndentTransID);
            var student = _studentRepository.PrintCertificate(grade.BatchMappingID);
            var contact = await _commonService.ContactInfo(string.Empty);
            //var branch = await _commonService.GetBranchDetails(student.TenantBranchID);
            /*Get tenant branch details*/
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var branch = commonRepository.GetBranchDetails(student.TenantBranchID);
            var name = contact.Where(x => x.ContactID == student.ContactID).FirstOrDefault();
            var totalMark = subjectDataSet.Tables[0].DataTablesToItem<SubjectMarks>();
            var totalSubject = subjectDataSet.Tables[1].DataTablesToList<PrintSubject>();
            int slNo = 1;
            int fail = 0;
            List<PrintSubject> subjects = new List<PrintSubject>();
            foreach (var subject in totalSubject) {
                PrintSubject print = subject.Mapper<PrintSubject>();
                print.SlNo = slNo;
                subjects.Add(print);
                if (subject.Result != "PASS") { fail++; }
                slNo++;
            }
            var image = await _commonService.GetDocument(new List<PrimaryKeyIDs> {
                new PrimaryKeyIDs { ContatcID = student.ContactID }
                });
            var doc = image.FirstOrDefault();
            MarksheetPrint marksheetPrint = new MarksheetPrint();
            marksheetPrint.BatchMappingID = grade.BatchMappingID;
            marksheetPrint.BranchName = branch.BranchName;
            marksheetPrint.StudentName = name.ContactName.ToUpper();
            marksheetPrint.RollNumber = student.RollNumber;
            marksheetPrint.ShortName = student.ShortName;
            marksheetPrint.Course = student.Course;
            marksheetPrint.Semester = grade.Semester;
            marksheetPrint.SemestersResult = (grade.Semester == student.TotalSemesters) ? "FINAL" : grade.Semester.ToString();
            DateTime endDate = DateTime.ParseExact(student.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            marksheetPrint.Date = endDate.ToString("MMMM-yyyy").ToUpper();
            //marksheetPrint.Address = branch.Address1 + "," + branch.Address2 + "," + branch.Area + "," + branch.District + "," + branch.State + "," + branch.Country + "," + branch.Pincode;
            marksheetPrint.Address = branch.Area + (string.IsNullOrWhiteSpace(branch.Area) ? "" : ", " + branch.Taluk) + ", " + branch.District + ", " + branch.Pincode;
            marksheetPrint.TotalTheoryMark = totalMark.TotalTheoryMarks;
            marksheetPrint.TotalPracticalMark = totalMark.TotalPracticalMarks;
            marksheetPrint.Result = fail == 0 ? "PASS" : "FAIL";
            marksheetPrint.DateOfIssue = grade.DateOfIssue;
            marksheetPrint.ProfileBase64 = doc?.ProfileBase64 ?? string.Empty;
            marksheetPrint.MIMEType = doc?.MIMEType ?? string.Empty;
            marksheetPrint.Subjects = subjects;

            return marksheetPrint;
        }
        public async Task<MarksheetPrint> PreviewMarksheetReprint(Guid examIndentTransID)
        {
            DataSet subjectDataSet = _studentRepository.PrintMarksheet(examIndentTransID);
            var grade = await _commonService.GetStudentGrade(examIndentTransID);
            var student = _studentRepository.PrintCertificate(grade.BatchMappingID);
            var contact = await _commonService.ContactInfo(string.Empty);
            //var branch = await _commonService.GetBranchDetails(student.TenantBranchID);
            /*Get tenant branch details*/
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var branch = commonRepository.GetBranchDetails(student.TenantBranchID);
            var name = contact.Where(x => x.ContactID == student.ContactID).FirstOrDefault();
            var totalMark = subjectDataSet.Tables[0].DataTablesToItem<SubjectMarks>();
            var totalSubject = subjectDataSet.Tables[1].DataTablesToList<PrintSubject>();
            int slNo = 1;
            int fail = 0;
            List<PrintSubject> subjects = new List<PrintSubject>();
            foreach (var subject in totalSubject)
            {
                PrintSubject print = subject.Mapper<PrintSubject>();
                print.SlNo = slNo;
                subjects.Add(print);
                if (subject.Result != "PASS") { fail++; }
                slNo++;
            }
            var image = await _commonService.GetDocument(new List<PrimaryKeyIDs> {
                new PrimaryKeyIDs { ContatcID = student.ContactID }
                });
            var doc = image.FirstOrDefault();
            MarksheetPrint marksheetPrint = new MarksheetPrint();
            marksheetPrint.BatchMappingID = grade.BatchMappingID;
            marksheetPrint.StudentName = name.ContactName;
            marksheetPrint.RollNumber = student.RollNumber;
            marksheetPrint.ShortName = student.ShortName;
            marksheetPrint.Course = student.Course;
            marksheetPrint.Semester = grade.Semester;
            marksheetPrint.SemestersResult = (grade.Semester == student.TotalSemesters) ? "Final" : grade.Semester.ToString();
            marksheetPrint.Date = student.EndDate;
            //marksheetPrint.Address = branch.Address1 + "," + branch.Address2 + "," + branch.Area + "," + branch.District + "," + branch.State + "," + branch.Country + "," + branch.Pincode;
            marksheetPrint.Address = branch.Area + (string.IsNullOrWhiteSpace(branch.Taluk) ? "" : ", " + branch.Taluk) + ", " + branch.District + ", " + branch.Pincode;
            marksheetPrint.TotalTheoryMark = totalMark.TotalTheoryMarks;
            marksheetPrint.TotalPracticalMark = totalMark.TotalPracticalMarks;
            marksheetPrint.Result = fail == 0 ? "PASS" : "FAIL";
            marksheetPrint.DateOfIssue = grade.DateOfIssue;
            marksheetPrint.ProfileBase64 = doc?.ProfileBase64 ?? string.Empty;
            marksheetPrint.MIMEType = doc?.MIMEType ?? string.Empty;
            marksheetPrint.Subjects = subjects;
            return marksheetPrint;
        }


        //public async Task<string> PreviewMarkUpdates(List<PreviewMarkUpdate> marks)
        //{
        //    var totalMark = marks.GroupBy(x => x.ExamIndentTransID)
        //                .Select(g => new
        //                {
        //                    ExamIndentTransID = g.Key,
        //                    TotalTheoryMarks = g.Sum(x => x.ObtainedTheoryMarks),
        //                    TotalPracticalMarks = g.Sum(x => x.ObtainedPracticalMarks)
        //                }).ToList();
        //    var totalSubject = marks.GroupBy(x => new { x.ExamIndentTransID, x.SubjectMappingID })
        //                            .Select(g => new
        //                            {
        //                                g.Key.ExamIndentTransID,
        //                                g.Key.SubjectMappingID,
        //                                SubjectName = g.Select( s => s.Subject),
        //                                TheoryMark = g.Select( s => s.ObtainedTheoryMarks),
        //                                PracticalMark = g.Select( s => s.ObtainedPracticalMarks),
        //                                Result = (g.Sum(s => s.ObtainedTheoryMarks) + g.Sum(s => s.ObtainedPracticalMarks)) >= _studentRepository.GetPassMark() ? "PASS" : "FAIL"
        //                            }).ToList();

        //    //var student = _studentRepository.PrintCertificate(marks.BatchMappingID);
        //    int slNo = 1;
        //    int fail = 0;
        //    List<PrintSubject> subjects = new List<PrintSubject>();
        //    foreach (var subject in totalSubject)
        //    {
        //        PrintSubject print = subject.Mapper<PrintSubject>();
        //        print.SlNo = slNo;
        //        subjects.Add(print);
        //        if (subject.Result != "PASS") { fail++; }
        //        slNo++;
        //    }
        //    return  "";
        //}
        /// <summary>
        /// Get students by batch wise
        /// </summary>
        /// <param name="courseBatchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<List<NewStudentByBatch>> GetStudentByBatchWise (Guid courseBatchID) {
            List<NewStudentByBatch> studentByBatches = new List<NewStudentByBatch>();
            var students = _studentRepository.GetStudentByBatchWise(courseBatchID);
            foreach (var student in students) {
                var contactInfo = await _commonService.ContactInfo(string.Empty);
                var contact = contactInfo.Where(x => x.ContactID == student.ContactID).FirstOrDefault();
                int totalSemester = await _commonService.GetCourseSemesterCount(student.BatchMappingID);
                int studentSemester = await _commonService.GetStudentSemesterCount(student.BatchMappingID);
                if (studentSemester != totalSemester && studentSemester < totalSemester) {
                    NewStudentByBatch studentByBatch = new NewStudentByBatch();
                    studentByBatch.BatchMappingID = student.BatchMappingID;
                    studentByBatch.StudentID = student.StudentID;
                    studentByBatch.ContactName = contact.ContactName;
                    studentByBatch.RollNumber = student.RollNumber;
                    if (studentSemester < totalSemester) {
                        studentByBatch.Semester = studentSemester + 1;
                    }
                    studentByBatches.Add(studentByBatch);
                }
            }
            return studentByBatches;
        }

        /// <summary>
        /// Print certificate and marksheet
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param name="printType"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public string PrintCertificateAndMarksheet (List<GetCertificateID> print) {
            int count;
            DataTable data = print.ToDataTable();
            count = _studentRepository.PrintCertificateAndMarksheet(data, _userIdentityInfo.ContactID);
            return count > 0 ? "Printed successfully" : throw new DawnUserException("Couldn't print");
        }

        /// <summary>
        /// Print hallticket
        /// </summary>
        /// <param name="examIndentID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<List<PrintHallTicket>> PrintHallTicket (Guid examIndentID) {
            var examDates = await _commonService.GetExamDate(examIndentID);
            var contacts = await _commonService.ContactInfo(string.Empty) ?? new List<ContactInfo>();
            var hallTickets = new List<PrintHallTicket>();
            foreach (var exam in examDates) {
                var student = _studentRepository.GetStudentHallTicket(exam.BatchMappingID);
                if (student == null)
                    continue;
                var image = await _commonService.GetDocument(new List<PrimaryKeyIDs> {
                new PrimaryKeyIDs { ContatcID = student.ContactID }
                });
                var doc = image.FirstOrDefault();
                var contact = contacts.FirstOrDefault(x => x.ContactID == student.ContactID);
                hallTickets.Add(new PrintHallTicket {
                    RollNumber = student.RollNumber,
                    StudentName = contact?.ContactName ?? string.Empty,
                    Course = student.Course,
                    BatchID = student.BatchNumber,
                    //ExamDate = DateTime.Parse(exam.ExamDate)
                    //       .ToString("dd-MM-yyyy"),//exam.ExamDate,
                    ExamDate = DateTime.ParseExact(exam.ExamDate,"dd-MM-yyyy hh:mm:ss tt",CultureInfo.InvariantCulture).ToString("dd-MM-yyyy"),
                    ProfileBase64 = doc?.ProfileBase64 ?? string.Empty,
                    MIMEType = doc?.MIMEType ?? string.Empty,
                    ShortName = student.ShortName,
                    CourseTypeName = student.CourseTypeName
                });
            }
            return hallTickets;
        }

        public async Task<Guid> CreateStudentEnquiry (CreateStudentEnquiry enquiry){
            if (enquiry == null)
                throw new DawnUserException("Invalid data");
            //if (_studentRepository.IsEmailExists(enquiry.Email))
            //    throw new DawnUserException("Email already exists.");
            if (_studentRepository.IsPhoneExists(enquiry.Phone)) 
                throw new DawnUserException("Phone number already exists.");
            //Check email otp code is verified
            if (!await _commonService.IsOTPVerified(enquiry.SMSOTPTransID))
                throw new DawnException("SMS OTP is not verified");
            DataTable table = enquiry.Address.ToDataTable();
            Guid studentEnquiryID = _studentRepository.CreateStudentEnquiry(enquiry, table, _userIdentityInfo.ContactID);
            if (studentEnquiryID == Guid.Empty)
                throw new DawnUserException("Couldn't register");
            TempDocumentActivate temp = new TempDocumentActivate();
            List<DocumentItem> documents = new List<DocumentItem>();
            if (enquiry.DocumentID != Guid.Empty)
            {
                documents.Add(new DocumentItem { DocumentID = enquiry.DocumentID, EntityID = studentEnquiryID });
                temp.DocumentItem = documents;
            }
            if (temp.DocumentItem != null)
            {
                var activateDocument = await _commonService.TempDocumentActivated(temp);
            }
            /*To Send SMS*/
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var franchiseeName = commonRepository.BranchDetail(enquiry.TenantBranchID);
            EnquirySMS enquirySMS = new EnquirySMS();
            enquirySMS.StudentName = enquiry.StudentName;
            enquirySMS.PhoneNumber = enquiry.Phone;
            enquirySMS.CourseName = _studentRepository.GetCourseName(enquiry.CourseID);//here course short name
            enquirySMS.FranchiseeName = franchiseeName.BranchName;
            try
            {
                await StudentEnquirySMS(enquirySMS);
            }
            catch (Exception ex)
            {
                AppLogger.Write($"Failed to Send StudentEnquirySMS  - {DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt fff")} for ContactId : {_userIdentityInfo.ContactID}, TenantID : {_userIdentityInfo.TenantID}, Message : {ex.Message}", "Error");
            }            
            return studentEnquiryID;
        }

        public async Task<List<GetStudentEnquiry>> GetEnquiryList (DateTime fromDate, DateTime toDate, string nameOrPhone) {
            var list = _studentRepository.GetEnquiryList(_userIdentityInfo.TenantBranchID, fromDate, toDate, nameOrPhone);
            List<GetStudentEnquiry> students = new List<GetStudentEnquiry>();
            foreach (var student in list) {
                GetStudentEnquiry getStudent = student.Mapper<GetStudentEnquiry>();
                getStudent.Address = _studentRepository.GetEnquiryAddress(student.StudentEnquiryID);
                var document = await _commonService.GetDocumentID(student.StudentEnquiryID, "Profile");
                getStudent.DocumentID = Guid.Parse(document);
                students.Add(getStudent);
            }
            return students;
        }

        public async Task<List<GetAlumniStudent>> GetAdmissionReport (string fromDate, string toDate) {
            List<GetAlumniStudent> studentList = new List<GetAlumniStudent>();
            var students = _studentRepository.GetAdmissionReport(fromDate, toDate, _userIdentityInfo.TenantBranchID);
            var contact = await _commonService.ContactInfo(string.Empty);
            var alumni = (from s in students
                          join c in contact on s.ContactID equals c.ContactID
                          select new GetAlumniStudent() {
                              StudentID = s.StudentID,
                              BatchMappingID = s.BatchMappingID,
                              RegisteredDate = s.RegisteredDate,
                              RollNumber = s.RollNumber,
                              StudentName = c.ContactName,
                              Course = s.Course,
                              StartDate = s.StartDate,
                              EndDate = s.EndDate,
                              StartTime = s.StartTime,
                              EndTime = s.EndTime,
                              Phone = c.Phone,
                              AlternatePhone = c.AlternatePhone,
                              Email = c.Email
                          }).ToList();

            return alumni;
        }

        public string StudentDiscontinued (CreateDiscontinued discontinued) {
            int count = _studentRepository.CheckBatchMapID(discontinued.BatchMappingID);
            if (count > 0)
                throw new DawnUserException("This student already discontinued");
            int result = _studentRepository.StudentDiscontinued(discontinued, _userIdentityInfo.ContactID);
            return result > 0 ? "Updated successfully" : throw new DawnUserException("Update failed");
        }

        public async Task<IEnumerable<NewMaterialPendingList>> GetMblMaterialPendingList (MaterialFilter filter) {
            List<NewMaterialPendingList> pendingLists = new List<NewMaterialPendingList>();
            IEnumerable<MaterialPendingList> student = GetMaterialIssuePending(filter);
            var contactInfo = await _commonService.ContactInfo(string.Empty);
            foreach (var item in student) {
                var contact = contactInfo.Where(x => x.ContactID == item.ContactID).FirstOrDefault();
                NewMaterialPendingList pendingList = new NewMaterialPendingList();
                int total = GetMaterialCount(item.BatchMappingID);
                int issued = GetIssuedMaterialCount(item.BatchMappingID);
                pendingList.StudentID = item.StudentID;
                pendingList.BatchMappingID = item.BatchMappingID;
                pendingList.StudentName = contact != null ? contact.ContactName : "";
                pendingList.RollNumber = item.RollNumber;
                pendingList.Course = item.Course;
                pendingList.BatchTime = item.BatchTime;
                pendingList.Total = total;
                pendingList.Issued = issued;
                pendingList.Pending = total - issued;
                pendingList.Materials = await MaterialList(item.BatchMappingID);
                if ((filter.Type.ToConditionFormat() == "PENDING") && issued != total) {
                    pendingLists.Add(pendingList);
                }
                else if ((filter.Type.ToConditionFormat() == "ISSUED") && issued == total) {
                    pendingLists.Add(pendingList);
                }
            }
            return pendingLists;
        }

        public async Task<List<Materials>> MaterialList (Guid batchMappingID) {
            DataSet courseDataSet = _studentRepository.GetStudentCourseDetail(batchMappingID);
            List<NewIssueMaterial> material = courseDataSet.Tables[0].DataTablesToList<NewIssueMaterial>();
            List<Materials> materials = new List<Materials>();
            List<MaterialNames> materialName = await _commonService.GetMaterialNameList();
            foreach (var item in material)
            {
                var name = materialName.Find(x => x.MaterialID == item.MaterialID);
                Materials material1 = new Materials();
                material1.MaterialID = item.MaterialID;
                material1.MaterialName = name != null ? name.MaterialName : "";
                material1.Semester = item.Semester;
                material1.IssuedDate = item.IssuedDate;
                material1.Status = item.IssuedDate == "" ? "Issue" : "Issued";
                materials.Add(material1);
            }
            return materials;
        }

        public async Task<IEnumerable<StudentMblList>> GetStudentMblList (StudentFilter filter) {
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            List<StudentContacts> contacts = commonRepository.GetStudentContacts(_userIdentityInfo.TenantBranchID);
            return _studentRepository.GetStudentMblList(filter, contacts);
        }

        public AdmissionStatistics GetAdmissionStatistics (int year) {
            DataSet dsAdmission = _studentRepository.GetAdmissionStatistics(year, _userIdentityInfo.TenantBranchID);
            AdmissionStatistics admission = dsAdmission.Tables[0].DataTablesToItem<AdmissionStatistics>();
            admission.MonthlyWise = dsAdmission.Tables[1].DataTablesToList<MonthlyStatistics>();
            return admission;
        }

        public AdmissionStatisticsByDate GetAdmissionStatisticsByDate (DateTime date) {
            return _studentRepository.GetAdmissionStatisticsByDate(date, _userIdentityInfo.TenantBranchID);
        }

        private async Task<string> GetConnectionString (Guid tenantId, string serviceName) {
            TenantServiceInfo tenantServiceInfo = new() {
                ServiceName = serviceName,
                TenantID = tenantId
            };
            dynamic dbAdminAPIResult = await _dawnHttpClient.PostAsync("dbadmin/get-tenant-constr-on-reg", tenantServiceInfo, false);
            string constring = dbAdminAPIResult.Code == 200 ? dbAdminAPIResult.Data.ToString() : "";
            return constring.ToDecrypt();
        }

        public async Task<List<StudentBasicInfo>> GetAllStudentByBranchWise () {
            List<StudentBasicInfo> studentList = new List<StudentBasicInfo>();
            var students = _studentRepository.GetAllStudentByBranchWise(_userIdentityInfo.TenantBranchID);
            var conatct = await _commonService.ContactInfo(string.Empty);
            var studentInfo = (from s in students
                               join c in conatct on s.ContactID equals c.ContactID
                               orderby c.ContactName ascending
                               select new StudentBasicInfo() {
                                   BatchMappingID = s.BatchMappingID,
                                   StudentID = s.StudentID,
                                   ContactID = c.ContactID,
                                   RollNumber = s.RollNumber,
                                   StudentName = c.ContactName,
                                   Course = s.Course,
                                   Phone = c.Phone,
                                   AlternatePhone = c.AlternatePhone,
                                   BatchNumber = s.BatchNumber,
                                   BatchTime = s.BatchTime,
                                   StartDate = s.StartDate,
                                   EndDate = s.EndDate,
                                   ShortName = s.ShortName
                               }).ToList();
            return studentInfo;
        }

        public async Task<List<CourseDetail>> MblFeesBalanceList (Guid? batchMappingID) {
            List<CourseDetail> feesBalance = new List<CourseDetail>();
            var balanceList = _studentRepository.MblFeesBalanceList(batchMappingID, _userIdentityInfo.TenantBranchID);
            var conatct = await _commonService.ContactInfo(string.Empty);
            foreach ( var balance in balanceList) {
                var contactInfo = conatct.Where(x => x.ContactID == balance.ContactID).FirstOrDefault();
                CourseDetail courseDetail = balance.Mapper<CourseDetail>();
                courseDetail.PaidFees = await _commonService.GetStudentPaidFees(balance.BatchMappingID);
                courseDetail.Balance = balance.TotalFees - courseDetail.PaidFees;
                courseDetail.StudentName = contactInfo?.ContactName ?? string.Empty;
                if(courseDetail.Balance > 0) {
                    feesBalance.Add(courseDetail);
                }
            }
            return feesBalance;
        }

        public string[] GetStatusDropdown () {
            string[] level = new string[] { "All", "Fees Balance", "Fees Completed" };
            return level;
        }
        public string[] GetStatusDropdownWeb()
        {
            string[] level = new string[] { "All", "Fees Balance", "Fees Completed" };
            return level;
        }
        public async Task<List<CertificateIndentTrack>> CertificateIndentTrack (string status, string? fromDate, string? toDate) {
            List<CertificateIndentTrack> indentTracks = new List<CertificateIndentTrack>();
            DataSet indentDataset = _studentRepository.CertificateIndentTrack(_userIdentityInfo.TenantBranchID, status, fromDate, toDate);
            var indentMaster = indentDataset.Tables[0].DataTablesToList<CertificateIndentTrack>();
            foreach(var item in indentMaster) {
                var indentTrans = indentDataset.Tables[1].DataTablesToList<CertificateIndentTrans>().Where(x => x.CertificateIndentID == item.CertificateIndentID).ToList();
                CertificateIndentTrack master = item.Mapper<CertificateIndentTrack>();
                master.IndentTrans = await GetCertificateTrans(indentTrans);
                indentTracks.Add(master);
            }
            return indentTracks;
        }

        public async Task<List<CertificateIndentTrans>> GetCertificateTrans(List<CertificateIndentTrans> indentTrans) {
            List<CertificateIndentTrans> transDetails = new List<CertificateIndentTrans>();
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Inventory");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var deliveryModes = commonRepository.GetDeliveryModeList();
            foreach (var item in indentTrans) {
                var examIndent = await _commonService.GetStudentCertificateIndentInfo(item.ExamIndentTransID);
                var courseInfo = await _commonService.GetStudentCourseInfo(examIndent.StudentBatchMapping);
                var contactInfo = await _commonService.ContactInfo(string.Empty);
                var contact = contactInfo.Where(x => x.ContactID == courseInfo.ContactID).FirstOrDefault();
                var deliverymode = deliveryModes.Where(x => x.DeliveryModeID.ToString().ToUpper() == item.DeliveryModeID.ToUpper()).FirstOrDefault();
                CertificateIndentTrans trans = new CertificateIndentTrans();
                trans.CertificateAndMarksheetID = item.CertificateAndMarksheetID;
                trans.ExamIndentTransID = item.ExamIndentTransID;
                trans.CertificateIndentID = item.CertificateIndentID;
                trans.ContactID = contact?.ContactID ?? Guid.Empty;
                trans.PrintType = item.PrintType;
                trans.Status = item.Status;
                trans.DeliveryModeID = item.DeliveryModeID;
                trans.DeliveryMode = deliverymode?.DeliveryModeName ?? string.Empty;
                trans.TrackingID = item.TrackingID;
                trans.StudentName = contact?.ContactName ?? string.Empty;
                trans.RollNumber = courseInfo.RollNumber;
                trans.Course = courseInfo.Course;
                trans.Semester = examIndent.Semester + "/" + (await _commonService.GetCourseSemesterCount(examIndent.StudentBatchMapping));
                trans.DespatchDate = item.DespatchDate;
                trans.ReceivedDate = item.ReceivedDate;
                trans.IssuedDate = item.IssuedDate;
                transDetails.Add(trans);
            }
            return transDetails;
        }

        public string RejectStudentEnquiry (RejectStudentEnquiry reject) {
            int result = _studentRepository.RejectStudentEnquiry(reject, _userIdentityInfo.ContactID);
            return result > 0 ? "Request closed." : throw new DawnUserException("Couldn't close the request.");
        }

        public async Task<GetEnquiryByStudent> GetEnquiryByStudent (Guid studentEnquiryID) {
            var student = _studentRepository.GetEnquiryByStudent(studentEnquiryID);
            student.Address = _studentRepository.GetEnquiryAddress(student.StudentEnquiryID);
            var document = await _commonService.GetDocumentID(student.StudentEnquiryID, "Profile");
            student.DocumentID = Guid.Parse(document);
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            student.BranchDetail = commonRepository.BranchDetail(student.TenantBranchID);
            return student;
        }

        public async Task<string> EditStudent (EditStudent student) {
            int count = _studentRepository.EditStudent(student, _userIdentityInfo.ContactID);
            if (count == 0)
                throw new DawnUserException("Updation failed");
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            int result = commonRepository.EditStudent(student, _userIdentityInfo.ContactID);
            TempDocumentActivate temp = new TempDocumentActivate();
            List<DocumentItem> documents = new List<DocumentItem>();
            if (student.DocumentID != Guid.Empty) {
                documents.Add(new DocumentItem { DocumentID = student.DocumentID, EntityID = student.ContactID });
                temp.DocumentItem = documents;
            }
            if (temp.DocumentItem != null) {
                var activateDocument = await _commonService.TempDocumentActivated(temp);
            }
            return result > 0 ? "Student information updated successfully." : throw new DawnUserException("Updation failed.");
        }

        public async Task<string> SendSuccessResponse (string email, string password, string name) {
            EmailSendRequest emailSend = new EmailSendRequest();
            emailSend.MailAddress = email;
            emailSend.EmailType = "RegistrationCompleted";
            emailSend.MessageParameter = new List<string> { name, email, password };
            string result = await _commonService.RegCompletedMail(emailSend);
            return result;
        }

        public async Task<List<MblPaymentHistory>> MblPaymentHistory (Guid batchMappingID) {
            List<MblPaymentHistory> histories = new List<MblPaymentHistory>();
            string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, connectionString);
            DataSet dsPayments = commonRepository.MblPaymentHistory(batchMappingID);
            var paymentMaster = dsPayments.Tables[0].DataTablesToList<MblPaymentHistory>();
            var paymentTransList = dsPayments.Tables[1].DataTablesToList<MblPaymentTrans>();
            foreach (var item in paymentMaster) {
                MblPaymentHistory history = item.Mapper<MblPaymentHistory>();
                var paymodes = paymentTransList.Where(x => x.TenantBranchTransID == item.TenantBranchTransID).ToList();
                foreach(var paymode in paymodes) {
                    var document = await _commonService.GetDocumentID(paymode.TransID, paymode.PayMode);
                    paymode.DocumentID = Guid.Parse(document);
                    history.Paymodes.Add(paymode);
                }
                histories.Add(history);
            }
            return histories;
        }

        public IEnumerable<ReasonDropdown> ReasonDropdown () {
            return _studentRepository.ReasonDropdown();
        }

        public async Task<IEnumerable<StudentEnquiryReport>> GetEnquiryReport (EnquiryFilter filter) {
            List<StudentEnquiryReport> reports = new List<StudentEnquiryReport>();
            var students = _studentRepository.GetEnquiryReport(_userIdentityInfo.TenantBranchID, filter);
            foreach(var student in students) {
                student.Address = _studentRepository.GetEnquiryAddress(student.StudentEnquiryID);
                var document = await _commonService.GetDocumentID(student.StudentEnquiryID, "Student");
                student.DocumentID = Guid.Parse(document);
                reports.Add(student);
            }
            return reports;
        }

        public string[] ReportStatusDropdown () {
            string[] status = new string[] { "All", "Pending", "Closed", "Registered" };
            return status;
        }

        public async Task<List<DocumentURLDetails>> GetStudentImage (Guid contactID) {
            List<PrimaryKeyIDs> primaryKeyIDs = new List<PrimaryKeyIDs>();
            PrimaryKeyIDs primaryKey = new PrimaryKeyIDs { ContatcID = contactID };
            primaryKeyIDs.Add(primaryKey);
            var image = await _commonService.GetDocument(primaryKeyIDs);
            return image;
        }

        public async Task<List<GetStudentIDCard>> GetStudentIDCard (Guid courseBatchID) {
            var students = _studentRepository.GetStudentIDCard(courseBatchID) ?? new List<GetStudentIDCard>();
            var contactInfo = await _commonService.ContactInfo(string.Empty) ?? new List<ContactInfo>();
            var branchDetail = await _commonService.GetBranchDetails(_userIdentityInfo.TenantBranchID);
            var franchisee = (await _commonService.GetFranchiseeID())?
                .FirstOrDefault(x => x.TenantBranchID == _userIdentityInfo.TenantBranchID);
            foreach (var student in students) {
                string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
                CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
                string studentPlace = commonRepository.GetStudentPlace(student.ContactID);
                var contact = contactInfo.FirstOrDefault(x => x.ContactID == student.ContactID);
                var image = await _commonService.GetDocument(new List<PrimaryKeyIDs> {
                new PrimaryKeyIDs { ContatcID = student.ContactID }
                });
                var doc = image.FirstOrDefault();
                student.ProfileBase64 = doc?.ProfileBase64 ?? string.Empty;
                student.MIMEType = doc?.MIMEType ?? string.Empty;
                student.StudentName = contact?.ContactName ?? string.Empty;
                student.Phone = contact?.Phone ?? string.Empty;
                student.FranchiseePlace = branchDetail.Area ?? string.Empty;
                student.FranchiseeID = franchisee?.FranchiseeCode ?? string.Empty;
                student.StudentPlace = studentPlace;
            }
            return students;
        }

        public IEnumerable<DropdownDetails> GetPurposeDropdown()
        {
            return _studentRepository.GetPurposeDropdown();
        }

        public IEnumerable<DropdownDetails> GetReferByDropdown()
        {
            return _studentRepository.GetReferByDropdown();
        }
        public async Task<StudentRegisterResult> RegisterStudentByPhone(RegisterStudent student)
        {
            //Check required registeration
            //await CheckUserRegister(student.Phone);
            //Contact Register
            return await UserRegister(student);
        }
        private async Task<StudentRegisterResult> UserRegister(RegisterStudent student)
        {
            string franchiseeNo = string.Empty;
            string addressResult = string.Empty;
            Guid studentContactID = Guid.Empty;
            try
            {
                var franchiseeIds = await _commonService.GetFranchiseeID();
                var franchiseeCode = franchiseeIds.Where(x => x.TenantBranchID == _userIdentityInfo.TenantBranchID).FirstOrDefault();
                franchiseeNo = franchiseeCode?.FranchiseeCode ?? "";
                RandomPassword randomPassword = new RandomPassword();
                string password = randomPassword.Next();
                //string password = "India_123";
                /*To get AuthRoleID from Tenant*/
                string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
                using StudentRepository studentRepository = new StudentRepository(_sqlI18N, _connectionString);
                List<GetAuthRoleID> authRoleIDs = studentRepository.GetAuthRoleID("TenantUser","Student");

                /*Register User*/
                StudentContactID result = _studentRepository.RegisterStudentNew(student, password, franchiseeNo, _userIdentityInfo.TenantBranchID, _userIdentityInfo.ContactID);
                studentContactID = result.ContactID;
                if (studentContactID == Guid.Empty)
                    throw new DawnUserException("Student Registration Error");

                /*Register student in Contact*/
                int studentRegister = studentRepository.RegisterStudentInContact(student, result.ContactID, _userIdentityInfo.TenantBranchID, _userIdentityInfo.ContactID);

                /*Insert student address details*/
                List<StudentAddress> address = student.Address;
                foreach (var addressItem in address)
                {
                    StudentAddress studentAddress = new StudentAddress();
                    studentAddress.ContactAddressID = Guid.NewGuid();
                    studentAddress.ContactID = result.ContactID;
                    studentAddress.AddressType = addressItem.AddressType;
                    studentAddress.Address1 = addressItem.Address1;
                    studentAddress.Address2 = addressItem.Address2;
                    studentAddress.Area = addressItem.Area;
                    studentAddress.District = addressItem.District;
                    studentAddress.State = addressItem.State;
                    studentAddress.Country = addressItem.Country;
                    studentAddress.Pincode = addressItem.Pincode;
                    studentAddress.Taluk = addressItem.Taluk;
                    //addressResult = await _commonService.CreateStudentAddress(studentAddress);
                    addressResult = studentRepository.CreateStudentAddress(studentAddress,_userIdentityInfo.ContactID) ? "Contact address updated successfully" : "Could not update the contact address";
                }
                /*Insert student images*/
                foreach (var documentItem in student.StudentImage)
                {
                    SaveDocument document = new SaveDocument();
                    document.DocumentID = Guid.NewGuid();
                    document.PrimaryKeyID = result.ContactID;
                    document.DocumentType = "Profile";
                    document.DocumentFile = documentItem;
                    string documentID = await _commonService.UploadDocument(document);
                    if (documentID == null || documentID == string.Empty)
                    {
                        RollbackStudentContact(studentContactID);
                        throw new DawnUserException("Student image upload failed");
                    }
                }
                /*User login create*/
                if (studentRegister == 1)
                    await _commonService.UserLoginCreate(new UserLoginCreate
                    {
                        ContactID = result.ContactID,
                        TenantID = _userIdentityInfo.TenantID,
                        TenantBranchID = _userIdentityInfo.TenantBranchID,
                        Password = password,
                        PhoneCode = student.PhoneCode,
                        Phone = student.Phone,
                        AuthRoleID = authRoleIDs.Select(x => x.AuthRoleID).ToArray() ?? new int[] { }
                    });
                /*Get Student enquired course name*/

                /*To send a password SMS for student*/
                
                //NotifySMS sms = new NotifySMS();
                //sms.StudentName = student.StudentName;
                //sms.CourseName = _studentRepository.GetCourseName(result.StudentID); //Get Student enquired course name
                //sms.FranchiseName = franchiseeCode.FranchiseeName;
                //sms.Password = password;
                //sms.PhoneNumber = student.Phone;
                //await StudentPasswordSMS(sms);

                return new StudentRegisterResult
                {
                    ContactID = result.ContactID,
                    StudentID = result.StudentID
                };
            }
            catch (Exception ex)
            {
                RollbackStudentContact(studentContactID);
                throw new DawnUserException(ex.Message);
            }
        }
        public async Task<bool> CertificateMarksheetSMS(Guid certificateAndMarksheetID)
        {
            /*Get ExamTransID*/
            ExamTransAndType examTransType = _studentRepository.GetExamTransID(certificateAndMarksheetID);
            
            string connInventory = await GetConnectionString(_userIdentityInfo.TenantID, "Inventory");
            using StudentRepository studentRepository = new StudentRepository(_sqlI18N, connInventory);            
            var studentBatchMappingID = studentRepository.GetStudentCertificateIndentInfo(examTransType.ExamIndentTransID);

            var getStudentCourseID = _studentRepository.GetStudentCourseID(studentBatchMappingID);

            string connTenant = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            using StudentRepository studentRepo = new StudentRepository(_sqlI18N, connTenant);
            var studentDetail = studentRepo.GetStudentDetail(getStudentCourseID.ContactID);

            CertificateMarksheetSMS certificateMarksheetSMS = new CertificateMarksheetSMS();
            certificateMarksheetSMS.CourseShortName = getStudentCourseID.ShortName;
            certificateMarksheetSMS.PhoneNumber = studentDetail.PhoneNumber;
            certificateMarksheetSMS.StudentName = studentDetail.StudentName;
            certificateMarksheetSMS.TemplateSysID = examTransType.PrintType == "Marksheet" ? "MARKSHEET ISSUED" : "CERTIFICATE ISSUED";
            await CertificateMarksheetSMS(certificateMarksheetSMS);

            return true;
        }
        public async Task<bool> MaterialIssuedSMS(IssueMaterial material)
        {
            string connTenant = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            using StudentRepository studentRepo = new StudentRepository(_sqlI18N, connTenant);
            var studentDetail = studentRepo.GetStudentDetail(material.ContactID);

            var subjectCourseName = _studentRepository.GetSubjectCourseName(material);
            MaterialSMS materialSMS = new MaterialSMS();
            materialSMS.StudentName = studentDetail.StudentName;
            materialSMS.PhoneNumber = studentDetail.PhoneNumber;
            materialSMS.Subject = subjectCourseName.SubjectName;
            materialSMS.CourseShortName = subjectCourseName.CourseShortName;
            await MaterialIssuedSMS(materialSMS);
            return true;
        }
        private async Task<bool> StudentPasswordSMS(PasswordSMS sms)
        {
            bool result;
            EntitiesDTO.Student.SMSQueue smsDetails = new EntitiesDTO.Student.SMSQueue();
            smsDetails.Token = _userIdentityInfo.BearerToken;
            smsDetails.DestinationNumber = sms.PhoneNumber ;
            smsDetails.TemplateSysID = "REGISTER";
            smsDetails.MessageParameter.Add($"{sms.StudentName}");  //studentName
            smsDetails.MessageParameter.Add($"{sms.CourseFranchiseeName}");   //[CourseName] //[FranchiseName] 
            smsDetails.MessageParameter.Add($"{sms.PhoneNumber}");   //[PhoneNumber] 
            smsDetails.MessageParameter.Add($"{sms.Password}");   //[Password] 
            result = await _commonService.NotifySMS(smsDetails);
            return result;
        }
        private async Task<bool> StudentPaymentSMS(PaymentSMS sms)
        {
            bool result;
            EntitiesDTO.Student.SMSQueue smsDetails = new EntitiesDTO.Student.SMSQueue();
            smsDetails.Token = _userIdentityInfo.BearerToken;
            smsDetails.DestinationNumber = sms.PhoneNumber ;
            smsDetails.TemplateSysID = "PAYMENT INTIMATION";
            smsDetails.MessageParameter.Add($"{sms.StudentName}");  
            smsDetails.MessageParameter.Add($"{sms.Amount}");   
            smsDetails.MessageParameter.Add($"{sms.CourseShortName}");   
            smsDetails.MessageParameter.Add($"{sms.PaymentMode}");
            //smsDetails.MessageParameter.Add($"{sms.PaymentType}");   
            result = await _commonService.NotifySMS(smsDetails);
            return result;
        }
        private async Task<bool> StudentEnquirySMS(EnquirySMS sms)
        {
            bool result;
            EntitiesDTO.Student.SMSQueue smsDetails = new EntitiesDTO.Student.SMSQueue();
            smsDetails.Token = _userIdentityInfo.BearerToken;
            smsDetails.DestinationNumber = sms.PhoneNumber;
            smsDetails.TemplateSysID = "ENQUIRY";
            smsDetails.MessageParameter.Add($"{sms.StudentName}");
            smsDetails.MessageParameter.Add($"{sms.CourseName}");
            smsDetails.MessageParameter.Add($"{sms.FranchiseeName}");
            result = await _commonService.NotifySMS(smsDetails);
            return result;
        }
        private async Task<bool> CertificateMarksheetSMS(CertificateMarksheetSMS sms)
        {
            bool result;
            EntitiesDTO.Student.SMSQueue smsDetails = new EntitiesDTO.Student.SMSQueue();
            smsDetails.Token = _userIdentityInfo.BearerToken;
            smsDetails.DestinationNumber = sms.PhoneNumber;
            smsDetails.TemplateSysID = sms.TemplateSysID;
            smsDetails.MessageParameter.Add($"{sms.StudentName}");
            smsDetails.MessageParameter.Add($"{sms.CourseShortName}");
            result = await _commonService.NotifySMS(smsDetails);
            return result;
        }
        private async Task<bool> MaterialIssuedSMS(MaterialSMS sms)
        {
            bool result;
            EntitiesDTO.Student.SMSQueue smsDetails = new EntitiesDTO.Student.SMSQueue();
            smsDetails.Token = _userIdentityInfo.BearerToken;
            smsDetails.DestinationNumber = sms.PhoneNumber;
            smsDetails.TemplateSysID = "MATERIALS ISSUED";
            smsDetails.MessageParameter.Add($"{sms.StudentName}");
            smsDetails.MessageParameter.Add($"{sms.Subject}");
            smsDetails.MessageParameter.Add($"{sms.CourseShortName}");
            result = await _commonService.NotifySMS(smsDetails);
            return result;
        }

        private async Task CheckUserRegister(string phoneNumber)
        {
            /*Check mobile number*/
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            using StudentRepository studentRepository = new StudentRepository(_sqlI18N, _connectionString);
            if(studentRepository.CheckStudentPhoneNumber(phoneNumber) >=1 )
                throw new DawnUserException("MobileNumber already exist for other student");
            //if (await _commonService.IsMobileExistCheck($"{student.PhoneCode}-{student.Phone}"))
            //    throw new DawnUserException(ContactMessage.MobileExists);
            //////Check email id 
            //if (await _commonService.IsEmailExistCheck(student.Email))
            //    throw new DawnUserException(ContactMessage.EmailExists);
        }
        public async Task<List<StudentLoginList>> GetStudentListByPhone(string phoneNumber)
        {
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            using StudentRepository studentRepository = new StudentRepository(_sqlI18N, _connectionString);
            return studentRepository.GetStudentListByPhone(phoneNumber);
        }
        private async void RollbackStudentContact(Guid contactID)
        {
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            using StudentRepository studentRepository = new StudentRepository(_sqlI18N, _connectionString);
            studentRepository.RollbackContact(contactID);  //Rollback in contact and address table

            _studentRepository.RollBackInStudent(contactID);  //Rollback in student table
        }
        #region OTP
        public async Task<EntitiesDTO.OTP.RegistrationOTPResult> RegistrationOTPRequest(EntitiesDTO.OTP.RegistrationOTPRequest otpRequest)
        {
            await CheckRequiredForOTPSend(otpRequest);
            EntitiesDTO.OTP.RegistrationOTPResult otpSendResult = new EntitiesDTO.OTP.RegistrationOTPResult();
            var smsResult = await SMSOTPRequest($"{otpRequest.PhoneCode}{otpRequest.Phone}");
            otpSendResult.OTPTransID = smsResult.OTPTransID;
            otpSendResult.ValidityTime = smsResult.ValidityTime;
            return otpSendResult;
        }
        private async Task CheckRequiredForOTPSend(EntitiesDTO.OTP.RegistrationOTPRequest otpRequest)
        {
            if (await _commonService.IsMobileExistCheck($"{otpRequest.PhoneCode}-{otpRequest.Phone}"))
                throw new DawnUserException(OTPMessage.MobileExists);            
        }
        private async Task<EntitiesDTO.OTP.OTPResult> SMSOTPRequest(string mobileNo)
        {
            var parameters = new List<string>();
            parameters.Add("Student");
            parameters.Add("");
            OTPResponse result = await _commonService.SendSMSOTPRequest(new OTPRequest
            {
                Phone = mobileNo,
                OTPType = "Registration Verification",
                //MessageParameter = new List<string> { "TCDS Computer Education", "", "OTPCode", "" }

                MessageParameter = parameters//new List<string> { "Student", "" }
            });
            EntitiesDTO.OTP.OTPResult otpResult = new EntitiesDTO.OTP.OTPResult();
            otpResult.OTPTransID = result.OTPTransID;
            otpResult.ValidityTime = result.ValidityTime;
            return otpResult;
        }
        public async Task<EntitiesDTO.OTP.OTPResult> SMSOTPResend(SMSResendRequest otpResend)
        {
            return await SMSOTPRequest($"{otpResend.PhoneCode}{otpResend.Phone}");
        }
        public async Task<EntitiesDTO.OTP.OTPVerifyResult> OTPVerify(EntitiesDTO.OTP.OTPVerify otpVerify)
        {
            EntitiesDTO.OTP.OTPVerifyResult oTPVerifyResult = new EntitiesDTO.OTP.OTPVerifyResult();
            string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "SMS");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, connectionString);
            var otpType = commonRepository.GetOTPType(otpVerify.SMSOTPTransID);
            try
            {
                EntitiesDTO.Admin.VerifyOTP verifySMSOTP = new EntitiesDTO.Admin.VerifyOTP();
                oTPVerifyResult.Message = OTPMessage.OTPVerifiedError;
                verifySMSOTP.OTPTransID = otpVerify.SMSOTPTransID;
                verifySMSOTP.OTPCode = otpVerify.SMSOTPCode;
                //verifySMSOTP.OTPType = "Registration Verification";
                verifySMSOTP.OTPType = otpType;
                oTPVerifyResult.IsMobileVerified = await _commonService.IsOTPChecked(otpVerify.SMSOTPTransID);
                if (!oTPVerifyResult.IsMobileVerified)
                    oTPVerifyResult.IsMobileVerified = await OTPVerify(verifySMSOTP);
                if (oTPVerifyResult.IsMobileVerified)
                {
                    oTPVerifyResult.IsOTPVerified = true;
                    oTPVerifyResult.Message = OTPMessage.OTPVerified;
                }
                else
                    oTPVerifyResult.Message = OTPMessage.OTPVerifiedError;

                return oTPVerifyResult;
            }
            catch (Exception)
            {
                return oTPVerifyResult;
            }
        }
        private async Task<bool> OTPVerify(EntitiesDTO.Admin.VerifyOTP otpVerify)
        {
            Guid otpVerificationID = await _commonService.OTPVerify(otpVerify);
            return otpVerificationID == Guid.Empty ? false : true;
        }
        public async Task<EntitiesDTO.OTP.RegistrationOTPResult> ForgotOTPRequest(EntitiesDTO.OTP.RegistrationOTPRequest otpRequest)
        {
            await CheckRequiredForForgotOTPSend(otpRequest);
            EntitiesDTO.OTP.RegistrationOTPResult otpSendResult = new EntitiesDTO.OTP.RegistrationOTPResult();
            var smsResult = await ForgotSMSOTPRequest($"{otpRequest.PhoneCode}{otpRequest.Phone}");
            otpSendResult.OTPTransID = smsResult.OTPTransID;
            otpSendResult.ValidityTime = smsResult.ValidityTime;
            return otpSendResult;
        }
        private async Task CheckRequiredForForgotOTPSend(EntitiesDTO.OTP.RegistrationOTPRequest otpRequest)
        {
            if (!await _commonService.IsMobileExistCheck($"{otpRequest.PhoneCode}-{otpRequest.Phone}"))
                throw new DawnUserException(OTPMessage.MobileNotExists);
        }
        private async Task<EntitiesDTO.OTP.OTPResult> ForgotSMSOTPRequest(string mobileNo)
        {
            var parameters = new List<string>();
            parameters.Add("Student");
            parameters.Add("");
            OTPResponse result = await _commonService.SendSMSOTPRequest(new OTPRequest
            {
                Phone = mobileNo,
                OTPType = "FORGOT PASSWORD",
                //MessageParameter = new List<string> { "TCDS Computer Education", "", "OTPCode", "" }
                MessageParameter = parameters//new List<string> { "Student", "" }
            });
            EntitiesDTO.OTP.OTPResult otpResult = new EntitiesDTO.OTP.OTPResult();
            otpResult.OTPTransID = result.OTPTransID;
            otpResult.ValidityTime = result.ValidityTime;
            return otpResult;
        }
        #endregion
        public async Task<List<MarkUpdatePreview>> CreateReprintStudentMarkUpdateNew(List<UpdateStudentMark> marks, bool preview)
        {
            if (marks == null)
                throw new DawnUserException("Invalid data");
            List<UpdateNewStudentMark> studentMarks = new List<UpdateNewStudentMark>();
            if (marks.Any(mark => (mark.ObtainedTheoryMarks > mark.TheoryMarks) || (mark.ObtainedPracticalMarks > mark.PracticalMarks)))
            {
                throw new DawnUserException("Mark cannot be greater than actual mark.");
            }
            foreach (var mark in marks)
            {
                UpdateNewStudentMark updateStudentMark = new UpdateNewStudentMark();
                updateStudentMark.ExamIndentTransID = mark.ExamIndentTransID;
                updateStudentMark.SubjectMappingID = mark.SubjectMappingID;
                updateStudentMark.BatchMappingID = mark.BatchMappingID;
                updateStudentMark.Semester = mark.Semester;
                updateStudentMark.TheoryMarks = mark.TheoryMarks;
                updateStudentMark.PracticalMarks = mark.PracticalMarks;
                updateStudentMark.ObtainedTheoryMarks = mark.ObtainedTheoryMarks;
                updateStudentMark.ObtainedPracticalMarks = mark.ObtainedPracticalMarks;
                updateStudentMark.Result = (updateStudentMark.ObtainedTheoryMarks + updateStudentMark.ObtainedPracticalMarks) >= _studentRepository.GetPassMark() ? "PASS" : "FAIL";
                studentMarks.Add(updateStudentMark);
            }
            //if (!preview)
            //{
            //    string indent = await CreateCertificateIndent(studentMarks);
            //}
            DataTable updateMark = studentMarks.ToDataTable();
            var result = _studentRepository.CreateReprintStudentMarkUpdateNew(updateMark, _userIdentityInfo.ContactID, preview);
            List<StudentTotalMarks> students = new List<StudentTotalMarks>();
            students.AddRange(result.Select(item => new StudentTotalMarks
            {
                ExamIndentTransID = item.ExamIndentTransID,
                TotalMark = item.TotalMark,
                ObtainedMark = item.ObtainedMark
            }));
            //string connInventory = await GetConnectionString(_userIdentityInfo.TenantID, "Inventory");
            //CommonRepository common = new CommonRepository(_sqlI18N, connInventory);
            //var updatedMark = common.UpdateStudentTotalMark(students.ToDataTable(), _userIdentityInfo.ContactID, preview);
            var previewResult = MarkUpdatesResult(marks);
            //List < MarkUpdatePreview > previewResult = new List<MarkUpdatePreview>();
            return previewResult;
        }
        public async Task<MarksheetPrint> RePrintMarksheetPreview(Guid examIndentTransID)
        {
            DataSet subjectDataSet = _studentRepository.RePrintMarksheet(examIndentTransID);
            var grade = await _commonService.GetStudentGrade(examIndentTransID);
            var student = _studentRepository.PrintCertificate(grade.BatchMappingID);
            var contact = await _commonService.ContactInfo(string.Empty);
            /*Get tenant branch details*/
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var branch = commonRepository.GetBranchDetails(student.TenantBranchID);
            var name = contact.Where(x => x.ContactID == student.ContactID).FirstOrDefault();
            var totalMark = subjectDataSet.Tables[0].DataTablesToItem<SubjectMarks>();
            var totalSubject = subjectDataSet.Tables[1].DataTablesToList<PrintSubject>();
            int slNo = 1;
            int fail = 0;
            List<PrintSubject> subjects = new List<PrintSubject>();
            foreach (var subject in totalSubject)
            {
                PrintSubject print = subject.Mapper<PrintSubject>();
                print.SlNo = slNo;
                subjects.Add(print);
                if (subject.Result != "PASS") { fail++; }
                slNo++;
            }
            var image = await _commonService.GetDocument(new List<PrimaryKeyIDs> {
                new PrimaryKeyIDs { ContatcID = student.ContactID }
                });
            var doc = image.FirstOrDefault();
            MarksheetPrint marksheetPrint = new MarksheetPrint();
            marksheetPrint.BatchMappingID = grade.BatchMappingID;
            marksheetPrint.StudentName = name.ContactName;
            marksheetPrint.RollNumber = student.RollNumber;
            marksheetPrint.ShortName = student.ShortName;
            marksheetPrint.Course = student.Course;
            marksheetPrint.Semester = grade.Semester;
            marksheetPrint.SemestersResult = (grade.Semester == student.TotalSemesters) ? "Final" : grade.Semester.ToString();
            marksheetPrint.Date = student.EndDate;
            marksheetPrint.Address = branch.Area + (string.IsNullOrWhiteSpace(branch.Taluk) ? "" : ", " + branch.Taluk) + ", " + branch.District + ", " + branch.Pincode;
            marksheetPrint.TotalTheoryMark = totalMark.TotalTheoryMarks;
            marksheetPrint.TotalPracticalMark = totalMark.TotalPracticalMarks;
            marksheetPrint.Result = fail == 0 ? "PASS" : "FAIL";
            marksheetPrint.DateOfIssue = DateTime.Now.ToString("dd-MM-yyyy");
            marksheetPrint.ProfileBase64 = doc?.ProfileBase64 ?? string.Empty;
            marksheetPrint.MIMEType = doc?.MIMEType ?? string.Empty;
            marksheetPrint.Subjects = subjects;

            return marksheetPrint;
        }
        public async Task<List<MarksheetPrint>> RePrintPreviewMarksheet(List<PreviewCretificateMarksheet> transID)
        {
            var marksheets = new List<MarksheetPrint>();
            foreach (var examIndent in transID)
            {
                var printMarksheet = await RePrintMarksheetPreview(examIndent.ExamIndentTransID);
                marksheets.Add(printMarksheet);
            }
            return marksheets;
        }
        public async Task<List<CertificatePrint>> RePrintPreviewCertificate(List<PreviewCretificateMarksheet> transID)
        {
            var certificate = new List<CertificatePrint>();
            foreach (var examIndent in transID)
            {
                var printCertificate = await RePrintCertificatePreview(examIndent.ExamIndentTransID);
                certificate.Add(printCertificate);
            }
            return certificate;
        }
        public async Task<CertificatePrint> RePrintCertificatePreview(Guid examIndentTransID)
        {
            DataSet subjectDataSet = _studentRepository.RePrintMarksheet(examIndentTransID);
            var grade = await _commonService.GetStudentGrade(examIndentTransID);
            var student = _studentRepository.PrintCertificate(grade.BatchMappingID);
            var contactInfo = await _commonService.ContactInfo(string.Empty);
            //var branch = await _commonService.GetBranchDetails(student.TenantBranchID);
            /*Get tenant branch details*/
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var branch = commonRepository.GetBranchDetails(student.TenantBranchID);
            var name = contactInfo.Where(x => x.ContactID == student.ContactID).FirstOrDefault();
            var image = await _commonService.GetDocument(new List<PrimaryKeyIDs> {
                new PrimaryKeyIDs { ContatcID = student.ContactID }
                });
            var doc = image.FirstOrDefault();
            var totalMark = subjectDataSet.Tables[0].DataTablesToItem<SubjectMarks>();
            string connInventory = await GetConnectionString(_userIdentityInfo.TenantID, "Inventory");
            CommonRepository common = new CommonRepository(_sqlI18N, connInventory);
            decimal percent = ((decimal)totalMark.ObtainedTotalMarks / totalMark.TotalMark) * 100;
            var studentGrade = common.GetStudentGrade(percent);

            CertificatePrint certificatePrint = new CertificatePrint();
            certificatePrint.BatchMappingID = grade.BatchMappingID;
            certificatePrint.StudentName = name?.ContactName ?? "";
            certificatePrint.RollNumber = student.RollNumber;
            certificatePrint.ShortName = student.ShortName;
            certificatePrint.Course = student.Course;
            certificatePrint.StartDate = student.StartDateMonthYear;
            certificatePrint.EndDate = student.EndDateMonthYear;
            certificatePrint.BranchName = branch.BranchName;
            certificatePrint.Address = branch.Area + (string.IsNullOrWhiteSpace(branch.Taluk) ? "" : ", " + branch.Taluk) + ", " + branch.District + ", " + branch.Pincode;
            certificatePrint.Grade = studentGrade;
            certificatePrint.DateOfIssue = DateTime.Now.ToString("dd-MM-yyyy");
            certificatePrint.ProfileBase64 = doc?.ProfileBase64 ?? string.Empty;
            certificatePrint.MIMEType = doc?.MIMEType ?? string.Empty;
            certificatePrint.CourseTypeName = student.CourseTypeName;
            certificatePrint.Subjects = _studentRepository.GetSubjectsName(grade.BatchMappingID);
            return certificatePrint;
        }
        public async Task<List<GetStudentIDCard>> GetStudentIDCardList(List<StudentIDCards> batchmapingIDs)
        {
            var batchMapingIds = batchmapingIDs.Select(a => new { a.BatchMappingID, ExamIndentTransID = Guid.Empty }).ToList().ToDataTable();//here ExamIndentTransID is nothing dummy declatation for SQL USER DEFINED TABLE DATATYPE
            var students = _studentRepository.GetStudentIDCardList(batchMapingIds) ?? new List<GetStudentIDCard>();
            var contactInfo = await _commonService.ContactInfo(string.Empty) ?? new List<ContactInfo>();
            var branchDetail = await _commonService.GetBranchDetails(_userIdentityInfo.TenantBranchID);
            var franchisee = (await _commonService.GetFranchiseeID())?
                .FirstOrDefault(x => x.TenantBranchID == _userIdentityInfo.TenantBranchID);


            string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var franchiseeRepository = new CommonRepository(_sqlI18N, connectionString);
            var tenantBranchIds = students.Select(a => new { a.TenantBranchID }).Distinct().ToList().ToDataTable();
            var branchDetails = franchiseeRepository.FranchiseeDetailsForIDCard(tenantBranchIds);

            foreach (var student in students)
            {
                string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
                CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
                string studentPlace = commonRepository.GetStudentPlace(student.ContactID);
                var contact = contactInfo.FirstOrDefault(x => x.ContactID == student.ContactID);
                var image = await _commonService.GetDocument(new List<PrimaryKeyIDs> {
                new PrimaryKeyIDs { ContatcID = student.ContactID }
                });                
                var doc = image.FirstOrDefault();
                student.ProfileBase64 = doc?.ProfileBase64 ?? string.Empty;
                student.MIMEType = doc?.MIMEType ?? string.Empty;
                student.StudentName = contact?.ContactName ?? string.Empty;
                student.Phone = contact?.Phone ?? string.Empty;
                //student.FranchiseePlace = branchDetail.Area ?? string.Empty;
                //student.FranchiseeID = franchisee?.FranchiseeCode ?? string.Empty;
                student.FranchiseePlace = branchDetails.Where( a => a.TenantBranchID == student.TenantBranchID).Select(a=> a.Area).FirstOrDefault() ?? string.Empty;
                student.FranchiseeID = branchDetails.Where(a => a.TenantBranchID == student.TenantBranchID).Select(a => a.FranchiseeCode).FirstOrDefault() ?? string.Empty;
                student.StudentPlace = studentPlace;
            }
            return students;
        }
        public async Task<string> EditStudentMarkRerequest(List<EditMark> editMarks)
        {
            foreach (EditMark editMark in editMarks)
            {
                if (!_studentRepository.CheckExamIndentTransIDForRePrint(editMark.ExamIndentTransID))
                    throw new Exception("Couldn't edit mark !");
                int count = _studentRepository.CheckMark(editMark.ExamIndentTransID, editMark.SubjectMappingID, editMark.ObtainedTheoryMarks, editMark.ObtainedPracticalMarks);
                if (count > 0)
                    throw new DawnUserException("Mark cannot be greater than actual mark");
            }
            DataTable marks = editMarks.ToDataTable();
            var totalMarks = _studentRepository.EditStudentMark(marks, _userIdentityInfo.ContactID);
            GetTotalMark getTotal = new GetTotalMark();
            getTotal.ExamIndentTransID = totalMarks.ExamIndentTransID;
            getTotal.TotalMarks = totalMarks.TotalMarks;
            int result = await _commonService.UpdateStudentEditedMark(getTotal);
            return result > 0 ? "Mark updated successfully !" : throw new Exception("Couldn't update mark !");
        }
        #endregion
    }
}