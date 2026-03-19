create or replace function public.app_fn_delete_user(p_id int)
    returns void
as
$$
declare
    v_cnt int;
begin
    delete from public.users where id = p_id;
    get diagnostics v_cnt = row_count;

    if v_cnt = 0 then
        raise exception 'user_not_found' using errcode = 'P0002';
    end if;
end;
$$ language plpgsql;
