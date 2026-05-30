CREATE TABLE IF NOT EXISTS categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE CHECK (LOWER(name) <> 'no category')
);

CREATE TABLE IF NOT EXISTS priorities (
    id SERIAL PRIMARY KEY,
    name VARCHAR(30) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS statuses (
    id SERIAL PRIMARY KEY,
    name VARCHAR(30) NOT NULL UNIQUE
);

INSERT INTO categories (name) VALUES ('Work'), ('Home'), ('Studying') ON CONFLICT (name) DO NOTHING;
INSERT INTO priorities (name) VALUES ('Low'), ('Medium'), ('High') ON CONFLICT (name) DO NOTHING;
INSERT INTO statuses (name) VALUES ('New'), ('In progress'), ('Done') ON CONFLICT (name) DO NOTHING;

CREATE TABLE IF NOT EXISTS tasks (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    description TEXT,
    category_id INT REFERENCES categories(id) ON DELETE SET NULL,
    priority_id INT REFERENCES priorities(id) ON DELETE RESTRICT,
    status_id INT REFERENCES statuses(id) ON DELETE RESTRICT,
    due_date DATE,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- Performance Indexes
CREATE INDEX IX_tasks_category_id ON tasks(category_id);
CREATE INDEX IX_tasks_priority_id ON tasks(priority_id);
CREATE INDEX IX_tasks_status_id ON tasks(status_id);
CREATE INDEX IX_tasks_created_at_desc ON tasks(created_at DESC); -- For optimizing GetAllAsync ordering
