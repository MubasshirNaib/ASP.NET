namespace Backend.Core.Exceptions
{
    public class EmailAlreadyExistsException : Exception
    {
        public EmailAlreadyExistsException(string email)
            : base($"Email {email} is already in use.")
        {
        }
    }

    public class AuthenticationFailedException : Exception
    {
        public AuthenticationFailedException(string message)
            : base(message)
        {
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string entity, Guid id)
            : base($"{entity} with ID {id} not found.")
        {
        }
    }
}