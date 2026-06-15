\restrict YW9NRenFXa6rCACz8IZ7sz8xBIviBHRMBRc1rCjoqftgimifR6dfQrwlyxNNnDf

-- Dumped from database version 17.9 (Homebrew)
-- Dumped by pg_dump version 17.9 (Homebrew)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: insert_01_mailqueue_event_and_return_id(text, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.insert_01_mailqueue_event_and_return_id(event_in text, aggregate_id uuid) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
inserted_id integer;
BEGIN
INSERT INTO events_01_MailQueue(event, aggregate_id, timestamp)
VALUES(event_in::text, aggregate_id,  now()) RETURNING id INTO inserted_id;
return inserted_id;
END;
$$;


--
-- Name: insert_01_user_event_and_return_id(text, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.insert_01_user_event_and_return_id(event_in text, aggregate_id uuid) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
inserted_id integer;
BEGIN
INSERT INTO events_01_User(event, aggregate_id, timestamp)
VALUES(event_in::text, aggregate_id,  now()) RETURNING id INTO inserted_id;
return inserted_id;
END;
$$;


--
-- Name: insert_md_01_mailqueue_aggregate_event_and_return_id(text, uuid, integer, text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.insert_md_01_mailqueue_aggregate_event_and_return_id(event_in text, aggregate_id uuid, distance_from_latest_snapshot integer, md text) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
inserted_id integer;
    event_id integer;
BEGIN
    event_id := insert_md_01_MailQueue_event_and_return_id(event_in, aggregate_id, distance_from_latest_snapshot, md);

INSERT INTO aggregate_events_01_MailQueue(aggregate_id, event_id)
VALUES(aggregate_id, event_id) RETURNING id INTO inserted_id;
return event_id;
END;
$$;


--
-- Name: insert_md_01_mailqueue_aggregate_event_and_return_id_opt_lock(text, uuid, integer, text, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.insert_md_01_mailqueue_aggregate_event_and_return_id_opt_lock(event_in text, aggregate_id uuid, distance_from_latest_snapshot integer, md text, last_event_id integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    inserted_id integer;
    event_id integer;
    found_last_event_id integer;
BEGIN
    SELECT id INTO found_last_event_id
    FROM events_01_MailQueue
    WHERE events_01_MailQueue.aggregate_id = insert_md_01_MailQueue_aggregate_event_and_return_id_opt_lock.aggregate_id
    ORDER BY id DESC LIMIT 1;

    IF last_event_id = 0 THEN
        IF found_last_event_id IS NOT NULL THEN
            RAISE EXCEPTION 'Optimistic locking check failed: expected no previous events, but found event %', found_last_event_id;
        END IF;
    ELSIF last_event_id > 0 THEN
        IF found_last_event_id IS NULL OR found_last_event_id <> last_event_id THEN
            RAISE EXCEPTION 'Optimistic locking check failed: expected last event id %, but found %', last_event_id, found_last_event_id;
        END IF;
    END IF;

    event_id := insert_md_01_MailQueue_event_and_return_id(event_in, aggregate_id, distance_from_latest_snapshot, md);

    INSERT INTO aggregate_events_01_MailQueue(aggregate_id, event_id)
    VALUES(aggregate_id, event_id) RETURNING id INTO inserted_id;
    return event_id;
END;
$$;


--
-- Name: insert_md_01_mailqueue_event_and_return_id(text, uuid, integer, text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.insert_md_01_mailqueue_event_and_return_id(event_in text, aggregate_id uuid, distance_from_latest_snapshot integer, md text) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
inserted_id integer;
BEGIN
INSERT INTO events_01_MailQueue(event, aggregate_id, distance_from_latest_snapshot, timestamp, md)
VALUES(event_in::text, aggregate_id, distance_from_latest_snapshot, now(), md) RETURNING id INTO inserted_id;
return inserted_id;
END;
$$;


--
-- Name: insert_md_01_user_aggregate_event_and_return_id(text, uuid, integer, text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.insert_md_01_user_aggregate_event_and_return_id(event_in text, aggregate_id uuid, distance_from_latest_snapshot integer, md text) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
inserted_id integer;
    event_id integer;
BEGIN
    event_id := insert_md_01_User_event_and_return_id(event_in, aggregate_id, distance_from_latest_snapshot, md);

INSERT INTO aggregate_events_01_User(aggregate_id, event_id)
VALUES(aggregate_id, event_id) RETURNING id INTO inserted_id;
return event_id;
END;
$$;


--
-- Name: insert_md_01_user_aggregate_event_and_return_id_opt_lock(text, uuid, integer, text, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.insert_md_01_user_aggregate_event_and_return_id_opt_lock(event_in text, aggregate_id uuid, distance_from_latest_snapshot integer, md text, last_event_id integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    inserted_id integer;
    event_id integer;
    found_last_event_id integer;
BEGIN
    SELECT id INTO found_last_event_id
    FROM events_01_User
    WHERE events_01_User.aggregate_id = insert_md_01_User_aggregate_event_and_return_id_opt_lock.aggregate_id
    ORDER BY id DESC LIMIT 1;

    IF last_event_id = 0 THEN
        IF found_last_event_id IS NOT NULL THEN
            RAISE EXCEPTION 'Optimistic locking check failed: expected no previous events, but found event %', found_last_event_id;
        END IF;
    ELSIF last_event_id > 0 THEN
        IF found_last_event_id IS NULL OR found_last_event_id <> last_event_id THEN
            RAISE EXCEPTION 'Optimistic locking check failed: expected last event id %, but found %', last_event_id, found_last_event_id;
        END IF;
    END IF;

    event_id := insert_md_01_User_event_and_return_id(event_in, aggregate_id, distance_from_latest_snapshot, md);

    INSERT INTO aggregate_events_01_User(aggregate_id, event_id)
    VALUES(aggregate_id, event_id) RETURNING id INTO inserted_id;
    return event_id;
END;
$$;


--
-- Name: insert_md_01_user_event_and_return_id(text, uuid, integer, text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.insert_md_01_user_event_and_return_id(event_in text, aggregate_id uuid, distance_from_latest_snapshot integer, md text) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
inserted_id integer;
BEGIN
INSERT INTO events_01_User(event, aggregate_id, distance_from_latest_snapshot, timestamp, md)
VALUES(event_in::text, aggregate_id, distance_from_latest_snapshot, now(), md) RETURNING id INTO inserted_id;
return inserted_id;
END;
$$;


--
-- Name: aggregate_events_01_mailqueue_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.aggregate_events_01_mailqueue_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: aggregate_events_01_mailqueue; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.aggregate_events_01_mailqueue (
    id integer DEFAULT nextval('public.aggregate_events_01_mailqueue_id_seq'::regclass) NOT NULL,
    aggregate_id uuid NOT NULL,
    event_id integer
);


--
-- Name: aggregate_events_01_user_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.aggregate_events_01_user_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: aggregate_events_01_user; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.aggregate_events_01_user (
    id integer DEFAULT nextval('public.aggregate_events_01_user_id_seq'::regclass) NOT NULL,
    aggregate_id uuid NOT NULL,
    event_id integer
);


--
-- Name: events_01_mailqueue; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.events_01_mailqueue (
    id integer NOT NULL,
    aggregate_id uuid NOT NULL,
    event text NOT NULL,
    published boolean DEFAULT false NOT NULL,
    "timestamp" timestamp without time zone NOT NULL,
    distance_from_latest_snapshot integer,
    md text
);


--
-- Name: events_01_mailqueue_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public.events_01_mailqueue ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.events_01_mailqueue_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: events_01_user; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.events_01_user (
    id integer NOT NULL,
    aggregate_id uuid NOT NULL,
    event text NOT NULL,
    published boolean DEFAULT false NOT NULL,
    "timestamp" timestamp without time zone NOT NULL,
    distance_from_latest_snapshot integer,
    md text
);


--
-- Name: events_01_user_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public.events_01_user ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.events_01_user_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: schema_migrations; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.schema_migrations (
    version character varying(128) NOT NULL
);


--
-- Name: snapshots_01_mailqueue_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.snapshots_01_mailqueue_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: snapshots_01_mailqueue; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.snapshots_01_mailqueue (
    id integer DEFAULT nextval('public.snapshots_01_mailqueue_id_seq'::regclass) NOT NULL,
    snapshot text NOT NULL,
    event_id integer,
    aggregate_id uuid NOT NULL,
    "timestamp" timestamp without time zone NOT NULL,
    is_deleted boolean DEFAULT false NOT NULL
);


--
-- Name: snapshots_01_user_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.snapshots_01_user_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: snapshots_01_user; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.snapshots_01_user (
    id integer DEFAULT nextval('public.snapshots_01_user_id_seq'::regclass) NOT NULL,
    snapshot text NOT NULL,
    event_id integer,
    aggregate_id uuid NOT NULL,
    "timestamp" timestamp without time zone NOT NULL,
    is_deleted boolean DEFAULT false NOT NULL
);


--
-- Name: aggregate_events_01_mailqueue aggregate_events_01_mailqueue_event_id_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.aggregate_events_01_mailqueue
    ADD CONSTRAINT aggregate_events_01_mailqueue_event_id_key UNIQUE (event_id);


--
-- Name: aggregate_events_01_mailqueue aggregate_events_01_mailqueue_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.aggregate_events_01_mailqueue
    ADD CONSTRAINT aggregate_events_01_mailqueue_pkey PRIMARY KEY (id);


--
-- Name: aggregate_events_01_user aggregate_events_01_user_event_id_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.aggregate_events_01_user
    ADD CONSTRAINT aggregate_events_01_user_event_id_key UNIQUE (event_id);


--
-- Name: aggregate_events_01_user aggregate_events_01_user_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.aggregate_events_01_user
    ADD CONSTRAINT aggregate_events_01_user_pkey PRIMARY KEY (id);


--
-- Name: events_01_mailqueue events_mailqueue_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.events_01_mailqueue
    ADD CONSTRAINT events_mailqueue_pkey PRIMARY KEY (id);


--
-- Name: events_01_user events_user_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.events_01_user
    ADD CONSTRAINT events_user_pkey PRIMARY KEY (id);


--
-- Name: schema_migrations schema_migrations_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.schema_migrations
    ADD CONSTRAINT schema_migrations_pkey PRIMARY KEY (version);


--
-- Name: snapshots_01_mailqueue snapshots_mailqueue_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.snapshots_01_mailqueue
    ADD CONSTRAINT snapshots_mailqueue_pkey PRIMARY KEY (id);


--
-- Name: snapshots_01_user snapshots_user_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.snapshots_01_user
    ADD CONSTRAINT snapshots_user_pkey PRIMARY KEY (id);


--
-- Name: ix_01_aggregate_events_mailqueue_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_01_aggregate_events_mailqueue_id ON public.aggregate_events_01_mailqueue USING btree (aggregate_id);


--
-- Name: ix_01_aggregate_events_user_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_01_aggregate_events_user_id ON public.aggregate_events_01_user USING btree (aggregate_id);


--
-- Name: ix_01_events_mailqueue_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_01_events_mailqueue_id ON public.events_01_mailqueue USING btree (aggregate_id);


--
-- Name: ix_01_events_mailqueue_timestamp; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_01_events_mailqueue_timestamp ON public.events_01_mailqueue USING btree ("timestamp");


--
-- Name: ix_01_events_user_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_01_events_user_id ON public.events_01_user USING btree (aggregate_id);


--
-- Name: ix_01_events_user_timestamp; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_01_events_user_timestamp ON public.events_01_user USING btree ("timestamp");


--
-- Name: ix_01_snapshot_mailqueue_aggregate_id_and_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_01_snapshot_mailqueue_aggregate_id_and_id ON public.snapshots_01_mailqueue USING btree (aggregate_id, id DESC);


--
-- Name: ix_01_snapshot_mailqueue_event_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_01_snapshot_mailqueue_event_id ON public.snapshots_01_mailqueue USING btree (event_id);


--
-- Name: ix_01_snapshot_mailqueue_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_01_snapshot_mailqueue_id ON public.snapshots_01_mailqueue USING btree (aggregate_id);


--
-- Name: ix_01_snapshot_user_aggregate_id_and_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_01_snapshot_user_aggregate_id_and_id ON public.snapshots_01_user USING btree (aggregate_id, id DESC);


--
-- Name: ix_01_snapshot_user_event_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_01_snapshot_user_event_id ON public.snapshots_01_user USING btree (event_id);


--
-- Name: ix_01_snapshot_user_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_01_snapshot_user_id ON public.snapshots_01_user USING btree (aggregate_id);


--
-- Name: ix_01_snapshots_mailqueue_timestamp; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_01_snapshots_mailqueue_timestamp ON public.snapshots_01_mailqueue USING btree ("timestamp");


--
-- Name: ix_01_snapshots_user_timestamp; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_01_snapshots_user_timestamp ON public.snapshots_01_user USING btree ("timestamp");


--
-- Name: aggregate_events_01_mailqueue aggregate_events_01_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.aggregate_events_01_mailqueue
    ADD CONSTRAINT aggregate_events_01_fk FOREIGN KEY (event_id) REFERENCES public.events_01_mailqueue(id) MATCH FULL ON DELETE CASCADE;


--
-- Name: aggregate_events_01_user aggregate_events_01_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.aggregate_events_01_user
    ADD CONSTRAINT aggregate_events_01_fk FOREIGN KEY (event_id) REFERENCES public.events_01_user(id) MATCH FULL ON DELETE CASCADE;


--
-- Name: snapshots_01_mailqueue event_01_mailqueue_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.snapshots_01_mailqueue
    ADD CONSTRAINT event_01_mailqueue_fk FOREIGN KEY (event_id) REFERENCES public.events_01_mailqueue(id) MATCH FULL ON DELETE CASCADE;


--
-- Name: snapshots_01_user event_01_user_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.snapshots_01_user
    ADD CONSTRAINT event_01_user_fk FOREIGN KEY (event_id) REFERENCES public.events_01_user(id) MATCH FULL ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict YW9NRenFXa6rCACz8IZ7sz8xBIviBHRMBRc1rCjoqftgimifR6dfQrwlyxNNnDf


--
-- Dbmate schema migrations
--

INSERT INTO public.schema_migrations (version) VALUES
    ('20260614110920'),
    ('20260615072901');
