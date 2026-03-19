create or replace function public.app_fn_update_user(
    p_id int,
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
as
$$
declare
    v_cnt int;
begin
    update public.users
    set password = p_password,
        name     = trim(p_name),
        surname  = trim(p_surname),
        age      = p_age
    where users.id = p_id;

    get diagnostics v_cnt = row_count;

    if v_cnt = 0 then
        raise exception 'user_not_found' using errcode = 'P0002';
    end if;

    return query
        select u.id, u.login, u.password, u.name, u.surname, u.age
        from public.users u
        where u.id = p_id;
end;
$$ language plpgsql;
