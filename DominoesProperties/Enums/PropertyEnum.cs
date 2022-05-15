using System;
namespace DominoesProperties.Enums
{
    public enum PropertyEnum
    {
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
        DEBIT,
        CREDIT
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
}
