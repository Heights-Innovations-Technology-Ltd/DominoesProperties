namespace DominoesProperties.Enums
{
    public enum PropertyEnum
    {
    }

    public enum Role
    {
        ADMIN = 2,
        SUPER = 1
    }

    public enum PropertyStatus
    {
        ONGOING_CONSTRUCTION,
        OPEN_FOR_INVESTMENT,
        CLOSED_FOR_INVESTMENT,
        RENTED_OUT
    }

    public enum Module
    {

    }

    public enum TransactionType
    {
        DR,
        CR
    }

    public enum Channel
    {
        CARD,
        WALLET,
        TRANSFER
    }

    public enum PaymentType
    {
        SUBSCRIPTION = 1,
        FUND_WALLET = 2,
        PROPERTY_PURCHASE = 3
    }

    public enum ValidationModule
    {
        RESET_PASSWORD,
        ACTIVATE_ACCOUNT
    }

    public enum EnquiryStatus
    {
        NEW,
        CLOSED
    }

    public enum UploadType
    {
        PICTURE,
        DOCUMENT
    }
}