create table if not exists public.users
(
    id       serial primary key,
    login    varchar(64)  not null unique,
    password varchar(128) not null,
    name     varchar(64)  not null,
    surname  varchar(64)  not null,
    age      int          not null check (age between 14 and 120)
);