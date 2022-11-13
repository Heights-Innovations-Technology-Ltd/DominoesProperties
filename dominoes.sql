create table if not exists HangfireAggregatedCounter
(
    Id       int auto_increment
        primary key,
    `Key`    varchar(100) not null,
    Value    int          not null,
    ExpireAt datetime     null,
    constraint IX_HangfireCounterAggregated_Key
        unique (`Key`)
)
    charset = utf8mb3;

create table if not exists HangfireCounter
(
    Id       int auto_increment
        primary key,
    `Key`    varchar(100) not null,
    Value    int          not null,
    ExpireAt datetime     null
)
    charset = utf8mb3;

create index IX_HangfireCounter_Key
    on HangfireCounter (`Key`);

create table if not exists HangfireDistributedLock
(
    Resource  varchar(100) not null,
    CreatedAt datetime(6)  not null
)
    charset = utf8mb3;

create table if not exists HangfireHash
(
    Id       int auto_increment
        primary key,
    `Key`    varchar(100) not null,
    Field    varchar(40)  not null,
    Value    longtext     null,
    ExpireAt datetime(6)  null,
    constraint IX_HangfireHash_Key_Field
        unique (`Key`, Field)
)
    charset = utf8mb3;

create table if not exists HangfireJob
(
    Id             int auto_increment
        primary key,
    StateId        int         null,
    StateName      varchar(20) null,
    InvocationData longtext    not null,
    Arguments      longtext    not null,
    CreatedAt      datetime(6) not null,
    ExpireAt       datetime(6) null
)
    charset = utf8mb3;

create index IX_HangfireJob_StateName
    on HangfireJob (StateName);

create table if not exists HangfireJobParameter
(
    Id    int auto_increment
        primary key,
    JobId int         not null,
    Name  varchar(40) not null,
    Value longtext    null,
    constraint IX_HangfireJobParameter_JobId_Name
        unique (JobId, Name),
    constraint FK_HangfireJobParameter_Job
        foreign key (JobId) references HangfireJob (Id)
            on update cascade on delete cascade
)
    charset = utf8mb3;

create table if not exists HangfireJobQueue
(
    Id         int auto_increment
        primary key,
    JobId      int         not null,
    FetchedAt  datetime(6) null,
    Queue      varchar(50) not null,
    FetchToken varchar(36) null
)
    charset = utf8mb3;

create index IX_HangfireJobQueue_QueueAndFetchedAt
    on HangfireJobQueue (Queue, FetchedAt);

create table if not exists HangfireJobState
(
    Id        int auto_increment
        primary key,
    JobId     int          not null,
    CreatedAt datetime(6)  not null,
    Name      varchar(20)  not null,
    Reason    varchar(100) null,
    Data      longtext     null,
    constraint FK_HangfireJobState_Job
        foreign key (JobId) references HangfireJob (Id)
            on update cascade on delete cascade
)
    charset = utf8mb3;

create table if not exists HangfireList
(
    Id       int auto_increment
        primary key,
    `Key`    varchar(100) not null,
    Value    longtext     null,
    ExpireAt datetime(6)  null
)
    charset = utf8mb3;

create table if not exists HangfireServer
(
    Id            varchar(100) not null
        primary key,
    Data          longtext     not null,
    LastHeartbeat datetime(6)  null
)
    charset = utf8mb3;

create table if not exists HangfireSet
(
    Id       int auto_increment
        primary key,
    `Key`    varchar(100) not null,
    Value    varchar(256) not null,
    Score    float        not null,
    ExpireAt datetime     null,
    constraint IX_HangfireSet_Key_Value
        unique (`Key`, Value)
)
    charset = utf8mb3;

create table if not exists HangfireState
(
    Id        int auto_increment
        primary key,
    JobId     int          not null,
    Name      varchar(20)  not null,
    Reason    varchar(100) null,
    CreatedAt datetime(6)  not null,
    Data      longtext     null,
    constraint FK_HangfireHangFire_State_Job
        foreign key (JobId) references HangfireJob (Id)
            on update cascade on delete cascade
)
    charset = utf8mb3;

create table if not exists applicationsettings
(
    Id           int auto_increment
        primary key,
    TestingMode  bit default b'1' not null,
    TestingEmail varchar(200)     not null,
    SettingName  varchar(200)     not null,
    constraint applicationsettings_Id_uindex
        unique (Id)
);

create table if not exists auditlog
(
    Id              bigint auto_increment
        primary key,
    RequestPayload  varchar(5000) null,
    ResponsePayload varchar(5000) null,
    CustomerId      bigint        null,
    Date            datetime      null,
    AdminId         bigint        null,
    constraint auditlog_Id_uindex
        unique (Id)
);

create table if not exists customer
(
    Id                   bigint auto_increment
        primary key,
    UniqueRef            varchar(50)                                      not null,
    FirstName            varchar(50)                                      not null,
    LastName             varchar(50)                                      not null,
    Email                varchar(200)                                     not null,
    Phone                varchar(14)                                      not null,
    Address              varchar(5000)                                    null,
    IsActive             bit           default b'1'                       null,
    IsVerified           bit           default b'0'                       not null,
    Password             varchar(5000) default 'Ul_*i3O2f9B9usJ_h9_ojg==' not null,
    IsSubscribed         bit           default b'0'                       not null,
    AccountNumber        varchar(10)                                      null,
    IsAccountVerified    bit           default b'0'                       not null,
    IsDeleted            bit           default b'0'                       not null,
    PassportUrl          varchar(500)                                     null,
    BankName             varchar(200)                                     null,
    DateRegistered       datetime      default CURRENT_TIMESTAMP          not null,
    NextSubscriptionDate datetime                                         null,
    PrevSubscriptionDate datetime                                         null,
    constraint customer_Email_uindex
        unique (Email),
    constraint customer_Id_uindex
        unique (Id),
    constraint customer_Phone_uindex
        unique (Phone),
    constraint customer_UniqueRef_uindex
        unique (UniqueRef)
);

create table if not exists description
(
    Id             bigint auto_increment
        primary key,
    PropertyId     varchar(200)     null,
    Bathroom       int default 0    null,
    Toilet         int default 0    null,
    FloorLevel     int default 0    null,
    Bedroom        int default 0    null,
    LandSize       varchar(50)      null,
    AirConditioned bit default b'0' null,
    Refrigerator   bit default b'0' null,
    Parking        bit default b'0' null,
    SwimmingPool   bit default b'0' null,
    Laundry        bit default b'0' null,
    Gym            bit default b'0' null,
    SecurityGuard  bit default b'0' null,
    Fireplace      bit default b'0' null,
    Basement       bit default b'0' null
);

create table if not exists emailretry
(
    id             bigint auto_increment
        primary key,
    category       varchar(50)                        not null,
    recipient      varchar(200)                       not null,
    subject        varchar(500)                       not null,
    recipient_name varchar(500)                       not null,
    body           text                               not null,
    date_created   datetime default CURRENT_TIMESTAMP not null,
    status_code    varchar(5)                         not null,
    retry_count    int      default 0                 not null
);

create table if not exists newsletter
(
    Id          bigint auto_increment
        primary key,
    Email       varchar(70)                        not null,
    DateCreated datetime default CURRENT_TIMESTAMP null,
    constraint newsletter_Email_uindex
        unique (Email)
);

create table if not exists paystackpayment
(
    Id              bigint auto_increment
        primary key,
    AccessCode      varchar(20)    not null,
    Amount          decimal(18, 2) not null,
    Channel         varchar(10)    null,
    FromAccount     varchar(10)    null,
    ToAccount       varchar(10)    null,
    Type            varchar(2)     not null,
    Date            datetime       null,
    Status          varchar(50)    null,
    TransactionRef  varchar(50)    not null,
    Payload         varchar(5000)  null,
    PaymentModule   varchar(50)    null,
    PaystackRef     varchar(50)    null,
    TransactionRef1 varchar(50)    null,
    constraint paystackpayment_Id_uindex
        unique (Id),
    constraint paystackpayment_pk
        unique (PaystackRef)
);

create index paystackpayment_TransactionRef_uindex
    on paystackpayment (TransactionRef);

create table if not exists property_type
(
    Id          int auto_increment
        primary key,
    Name        varchar(200) not null,
    DateCreated date         null,
    constraint property_type_Id_uindex
        unique (Id)
);

create table if not exists role
(
    Id          int auto_increment
        primary key,
    RoleName    varchar(50)   null,
    CreatedBy   varchar(200)  null,
    Privilege   varchar(5000) null,
    Page        varchar(5000) null,
    DateCreated datetime      null
);

create table if not exists admin
(
    Email       varchar(200)     not null
        primary key,
    Password    varchar(1000)    not null,
    RoleFK      int default 1    not null,
    IsActive    bit default b'1' not null,
    IsDeleted   bit default b'0' not null,
    DateCreated datetime         null,
    CreatedBy   varchar(200)     not null,
    constraint admin_Email_uindex
        unique (Email),
    constraint admin_role_Id_fk
        foreign key (RoleFK) references role (Id)
            on update cascade on delete set default
);

create table if not exists blogpost
(
    Id           bigint                             not null
        primary key,
    BlogTitle    varchar(200)                       null,
    BlogContent  text                               null,
    CreatedOn    datetime default CURRENT_TIMESTAMP null,
    CreatedBy    varchar(70)                        null,
    BlogTags     varchar(500)                       null,
    BlogImage    varchar(500)                       null,
    IsDeleted    bit      default b'0'              not null,
    UniqueNumber varchar(200)                       null,
    constraint blogpost_admin_null_fk
        foreign key (CreatedBy) references admin (Email)
);

create table if not exists enquiry
(
    Id                      bigint auto_increment
        primary key,
    CustomerUniqueReference varchar(50)                           not null,
    PropertyReference       varchar(50)                           not null,
    Subject                 varchar(100)                          not null,
    Message                 text                                  not null,
    DateCreated             datetime    default CURRENT_TIMESTAMP not null,
    Status                  varchar(10) default 'NEW'             not null,
    ClosedBy                varchar(200)                          null,
    constraint enquiry_admin_Email_fk
        foreign key (ClosedBy) references admin (Email)
);

create index enquiry_Status_index
    on enquiry (Status);

create table if not exists property
(
    Id                       bigint auto_increment
        primary key,
    UniqueId                 varchar(50)                                   not null,
    Name                     varchar(200)                                  not null,
    Location                 varchar(500)                                  null,
    Type                     int                                           not null,
    TotalUnits               int            default 0                      not null,
    UnitPrice                decimal(18, 2) default 0.00                   not null,
    TotalPrice               decimal(18, 2) default 0.00                   not null,
    Status                   varchar(50)    default 'ONGOING_CONSTRUCTION' not null,
    UnitSold                 int            default 0                      not null,
    UnitAvailable            int            default 0                      not null,
    ClosingDate              date                                          null,
    TargetYield              decimal(18, 2) default 0.00                   not null,
    ProjectedGrowth          decimal(18, 2) default 0.00                   not null,
    InterestRate             decimal(18, 2) default 0.00                   not null,
    DateCreated              datetime       default CURRENT_TIMESTAMP      not null,
    Longitude                varchar(100)                                  null,
    Latitude                 varchar(100)                                  null,
    CreatedBy                varchar(200)                                  not null,
    IsDeleted                bit            default b'0'                   not null,
    Summary                  text                                          not null,
    Account                  varchar(10)                                   null,
    Bank                     varchar(100)                                  null,
    MaxUnitPerCustomer       int            default 1000                   not null,
    VideoLink                varchar(500)                                  null,
    AllowSharing             bit            default b'0'                   not null,
    MinimumSharingPercentage int            default 0                      null,
    constraint property_Id_uindex
        unique (Id),
    constraint property_UniqueId_uindex
        unique (UniqueId),
    constraint property_admin_Email_fk
        foreign key (CreatedBy) references admin (Email),
    constraint property_property_type_Id_fk
        foreign key (Type) references property_type (Id)
);

create table if not exists investment
(
    Id                   bigint auto_increment
        primary key,
    CustomerId           bigint                           not null,
    PropertyId           bigint                           not null,
    Units                int                              not null,
    PaymentDate          datetime                         not null,
    Amount               decimal(18, 2)                   not null,
    Yield                decimal(18, 2)                   null,
    PaymentType          varchar(20)                      not null,
    YearlyInterestAmount decimal(18, 2)                   null,
    TransactionRef       varchar(50)                      not null,
    Status               varchar(10)    default 'PENDING' not null,
    UnitPrice            decimal(18, 2) default 0.00      null,
    constraint investment_Id_uindex
        unique (Id),
    constraint investment_customer_Id_fk
        foreign key (CustomerId) references customer (Id),
    constraint investment_property_Id_fk
        foreign key (PropertyId) references property (Id)
            on update cascade
);

create index investment_Status_index
    on investment (Status);

create index investment_transaction_TransactionRef_fk
    on investment (TransactionRef);

create table if not exists offline_investment
(
    Id          bigint auto_increment
        primary key,
    CustomerId  bigint                           not null,
    PropertyId  bigint                           not null,
    Units       int                              not null,
    PaymentDate datetime                         null,
    Amount      decimal(18, 2)                   not null,
    PaymentRef  varchar(50)                      not null,
    Status      varchar(10)    default 'PENDING' not null,
    UnitPrice   decimal(18, 2) default 0.00      null,
    CreatedDate datetime                         not null,
    ProofUrl    varchar(250)                     null,
    Comment     text                             null,
    TreatedDate datetime                         null,
    TreatedBy   varchar(70)                      null,
    constraint off_investment_Id_uindex
        unique (Id),
    constraint off_investment_customer_Id_fk
        foreign key (CustomerId) references customer (Id),
    constraint off_investment_property_Id_fk
        foreign key (PropertyId) references property (Id)
            on update cascade
);

create index investment_Payment_TransactionRef_fk
    on offline_investment (PaymentRef);

create index off_investment_Status_index
    on offline_investment (Status);

create table if not exists propertyuploads
(
    Id           bigint auto_increment
        primary key,
    PropertyId   bigint                                not null,
    Url          varchar(500)                          not null,
    ImageName    varchar(100)                          not null,
    DateUploaded datetime    default CURRENT_TIMESTAMP not null,
    UploadType   varchar(10) default 'PICTURE'         not null,
    AdminEmail   varchar(200)                          not null,
    constraint propertyuploads_Id_uindex
        unique (Id),
    constraint propertyuploads_Url_uindex
        unique (Url),
    constraint propertyuploads_admin_Email_fk
        foreign key (AdminEmail) references admin (Email),
    constraint propertyuploads_property_Id_fk
        foreign key (PropertyId) references property (Id)
);

create table if not exists sharinggroup
(
    UniqueId             varchar(32)                              not null
        primary key,
    Alias                varchar(50)                              not null,
    PropertyId           bigint                                   not null,
    MaxCount             int                                      not null,
    CustomerUniqueId     varchar(50)                              null,
    Date                 datetime       default CURRENT_TIMESTAMP not null,
    IsClosed             bit            default b'0'              not null,
    PercentageSubscribed int            default 0                 not null,
    UnitPrice            decimal(18, 2) default 0.00              null,
    constraint sharinggroup_UniqueId_uindex
        unique (UniqueId),
    constraint sharinggroup_property_Id_fk
        foreign key (PropertyId) references property (Id)
);

create table if not exists sharingentry
(
    Id               bigint auto_increment
        primary key,
    GroupId          varchar(32)                        not null,
    CustomerId       bigint                             not null,
    IsClosed         bit      default b'0'              not null,
    PercentageShare  int                                not null,
    Date             datetime default CURRENT_TIMESTAMP not null,
    PaymentReference varchar(50)                        null,
    IsReversed       bit      default b'0'              null,
    constraint sharingentry_Id_uindex
        unique (Id),
    constraint sharingentry_PaymentReference_uindex
        unique (PaymentReference),
    constraint sharingentry_customer_Id_fk
        foreign key (CustomerId) references customer (Id),
    constraint sharingentry_sharinggroup_UniqueId_fk
        foreign key (GroupId) references sharinggroup (UniqueId)
            on update cascade
);

create index sharingentry_CustomerId_index
    on sharingentry (CustomerId);

create index sharingentry_GroupId_index
    on sharingentry (GroupId desc);

create index sharingentry_IsClosed_index
    on sharingentry (IsClosed);

create table if not exists transaction
(
    TransactionRef  varchar(50)                        not null
        primary key,
    Amount          decimal(18, 2)                     null,
    CustomerId      bigint                             not null,
    TransactionDate datetime default CURRENT_TIMESTAMP not null,
    Module          varchar(50)                        null,
    TransactionType varchar(10)                        null,
    Status          varchar(50)                        null,
    Channel         varchar(10)                        null,
    RequestPayload  varchar(5000)                      null,
    ResponsePayload varchar(5000)                      null,
    constraint transaction_customer_Id_fk
        foreign key (CustomerId) references customer (Id)
);

create index transaction_Status_index
    on transaction (Status);

create index transaction_TransactionRef_index
    on transaction (TransactionRef);

create table if not exists wallet
(
    Id                    int auto_increment
        primary key,
    WalletNo              varchar(40)    not null,
    CustomerId            bigint         not null,
    `Limit`               decimal(18, 2) null,
    Balance               decimal(18, 2) null,
    LastTransactionDate   datetime       null,
    LastTransactionAmount decimal(18, 2) null,
    DateCreated           datetime       null,
    constraint wallet_CustomerId_uindex
        unique (CustomerId),
    constraint wallet_Id_uindex
        unique (Id),
    constraint wallet_WalletNo_uindex
        unique (WalletNo),
    constraint wallet_customer_Id_fk
        foreign key (CustomerId) references customer (Id)
);

