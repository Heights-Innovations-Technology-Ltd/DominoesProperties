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
        ONGOING_CONSTRUCTION=0,
        OPEN_FOR_INVESTMENT=1,
        CLOSED_FOR_INVESTMENT=2,
        RENTED_OUT=3
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
        PROPERTY_PURCHASE = 3,
        PROPERTY_PAIRING_GROUP = 4,
        PROPERTY_PAIRING = 5,
        REVERSAL = 6
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
        DOCUMENT,
        COVER
    }
}