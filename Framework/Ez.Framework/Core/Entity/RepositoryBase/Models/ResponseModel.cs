using Ez.Framework.Utilities;
using System;
using System.Data.Entity.Validation;

namespace Ez.Framework.Core.Entity.RepositoryBase.Models
{
    /// <summary>
    /// General response model
    /// </summary>
    public class ResponseModel
    {
        #region Constructors

        public ResponseModel()
        {

        }

        public ResponseModel(DbEntityValidationException exception)
        {
            Success = false;
            Message = BuildEntityValidationError(exception);
            DetailMessage = BuildEntityValidationError(exception);
        }

        public ResponseModel(Exception exception)
        {
            Success = false;
            var innerEx = exception;
            while (innerEx.InnerException != null)
            {
                innerEx = innerEx.InnerException;
            }

            Message = innerEx.Message;
            DetailMessage = innerEx.BuildErrorMessage();
        }

        #endregion

        #region Public Properties
        public bool Success { get; set; }

        public string Message { get; set; }

        public string DetailMessage { get; set; }

        public object Data { get; set; }

        public ResponseStatusEnums ResponseStatus { get; set; }

        #endregion

        public ResponseModel SetMessage(string message)
        {
            Message = message;
            return this;
        }

        private string BuildEntityValidationError(DbEntityValidationException exception)
        {
            var message = string.Empty;
            foreach (var eve in exception.EntityValidationErrors)
            {
                message += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                foreach (var ve in eve.ValidationErrors)
                {
                    message += string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                }
            }
            return message;
        }

        public static string BuildDetailMessage(Exception exception)
        {
            return
                string.Format(
                    @"<div class='error-item'><div class='error-title'>{0}</div>
                                <div class='error-source'><strong>Source:</strong> {1}</div>
                                <div class='error-stack-trace'><strong>Stack Trace:</strong> {2}</div></div>",
                    exception.Message, exception.Source, exception.StackTrace);
        }

        public static string BuildDetailMessage(ExceptionInformation exception)
        {
            return
                string.Format(
                    @"<div class='error-item'><div class='error-title'>{0}</div>
                                <div class='error-source'><strong>Source:</strong> {1}</div>
                                <div class='error-stack-trace'><strong>Stack Trace:</strong> {2}</div></div>",
                    exception.Message, exception.Source, exception.StackTrace);
        }
    }

    public class ExceptionInformation
    {
        public ExceptionInformation()
        {

        }

        public ExceptionInformation(Exception exception)
            : this()
        {
            Message = exception.Message;
            Source = exception.Source;
            StackTrace = exception.StackTrace;
        }

        #region Public Properties

        public string Message { get; set; }

        public string Source { get; set; }

        public string StackTrace { get; set; }

        #endregion
    }

    public enum ResponseStatusEnums
    {
        Success = 1,
        Warning = 2,
        Error = 3,
        AccessDenied = 4
    }
}
