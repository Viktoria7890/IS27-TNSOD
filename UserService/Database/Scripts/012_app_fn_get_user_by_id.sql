create or replace function public.app_fn_get_user_by_id(p_id int)
    returns table
            (
                id       int,
                login    varchar,
                password varchar,
                name     varchar,
                surname  varchar,
                age      int
            )
    language sql
as
$$
select u.id, u.login, u.password, u.name, u.surname, u.age
from public.users u
where u.id = p_id;
$$;
