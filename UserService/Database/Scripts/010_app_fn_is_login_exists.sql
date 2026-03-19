create or replace function public.app_fn_is_login_exists(p_login varchar)
    returns boolean
    language sql
as
$$
select exists (select 1
               from public.users u
               where u.login = p_login);
$$;
