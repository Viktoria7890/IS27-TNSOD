create or replace function public.app_fn_get_users_by_name(
    p_name varchar,
    p_surname varchar
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
        select u.id, u.login, u.password, u.name, u.surname, u.age
        from public.users u
        where u.name = p_name
          and u.surname = p_surname
        order by u.id;
end;
$$;
