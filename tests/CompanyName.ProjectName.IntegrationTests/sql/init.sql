CREATE TABLE IF NOT EXISTS users (
    id          UUID              NOT NULL,
    email       VARCHAR(256)      NOT NULL,
    name        VARCHAR(100)      NOT NULL,
    role        VARCHAR(20)       NOT NULL DEFAULT 'User',
    created_at  TIMESTAMPTZ       NOT NULL DEFAULT now(),
    CONSTRAINT pk_users PRIMARY KEY (id)
);

CREATE UNIQUE INDEX IF NOT EXISTS ix_users_email ON users (email);
