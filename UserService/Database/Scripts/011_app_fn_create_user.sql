create or replace function public.app_fn_create_user(
    p_login varchar,
    p_password varchar,
    p_name varchar,
    p_surname varchar,
    p_age int
)
    returns table
            (
                id       int,
                login    varchar,
                password varchar,
                name     varchar,
                surname  varchar,
                age      int
            )
    language plpgsql
as
$$
begin
    return query
        insert into public.users (login, password, name, surname, age)
            values (trim(p_login),
                    p_password,
                    trim(p_name),
                    trim(p_surname),
                    p_age)
            returning users.id, users.login, users.password, users.name, users.surname, users.age;

exception
    when unique_violation then
        raise exception 'login_not_unique' using errcode = '23505';
end;
$$;
