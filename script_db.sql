CREATE TABLE roles (
    id serial primary key,
    name VARCHAR NOT NULL
);

CREATE TABLE users (
    id serial primary key,
    email VARCHAR NOT NULL,
    password VARCHAR NOT NULL,
    last_name VARCHAR,
    first_name VARCHAR,
    middle_name VARCHAR,
    role_id int REFERENCES roles(id),
    is_active BOOLEAN,
    created_at TIMESTAMP,
    updated_at TIMESTAMP,
    attempt INTEGER,
    salt VARCHAR
);

CREATE TABLE wallets (
    id serial primary key,
    user_id int NOT NULL REFERENCES users(id),
    balance DECIMAL
);

CREATE TABLE confirmation_codes (
    id serial primary key,
    user_id int NOT NULL REFERENCES users(id),
    code VARCHAR NOT NULL,
    created_at TIMESTAMP,
    expires_at TIMESTAMP NOT NULL
);

CREATE TABLE password_resets (
    id serial primary key,
    user_id int NOT NULL REFERENCES users(id),
    token VARCHAR NOT NULL,
    created_at TIMESTAMP,
    expires_at TIMESTAMP NOT NULL
);

CREATE TABLE telegrams (
    id serial primary key,
    user_id int REFERENCES users(id),
    tg_id BIGINT NOT NULL,
    linked_at TIMESTAMP
);

CREATE TABLE user_bans (
    id serial primary key,
    user_id int NOT NULL REFERENCES users(id),
    start_date TIMESTAMP NOT NULL,
    end_date TIMESTAMP,
    reason VARCHAR
);

CREATE TABLE addresses (
    id serial primary key,
    user_id int NOT NULL REFERENCES users(id),
    city VARCHAR,
    street VARCHAR,
    house VARCHAR,
    apartment VARCHAR
);

CREATE TABLE entrepreneurs (
    id serial primary key,
    user_id int NOT NULL REFERENCES users(id),
    name VARCHAR,
    account_number VARCHAR,
    wallet_id int REFERENCES wallets(id),
    is_active BOOLEAN,
    created_at TIMESTAMP,
    updated_at TIMESTAMP
);

CREATE TABLE categories (
    id serial primary key,
    name VARCHAR NOT NULL,
    parent_id int REFERENCES categories(id),
    level INTEGER
);

CREATE TABLE characteristics (
    id serial primary key,
    name VARCHAR NOT NULL,
    unit VARCHAR
);

CREATE TABLE products (
    id serial primary key,
    entrepreneur_id int NOT NULL REFERENCES entrepreneurs(id),
    category_id int REFERENCES categories(id),
    name VARCHAR NOT NULL,
    description TEXT,
    price DECIMAL NOT NULL,
    stock INTEGER,
    is_active BOOLEAN,
    rating DECIMAL,
    reviews_count INTEGER,
    created_at TIMESTAMP,
    updated_at TIMESTAMP
);

CREATE TABLE product_characteristics (
    id serial primary key,
    product_id int NOT NULL REFERENCES products(id),
    characteristic_id int NOT NULL REFERENCES characteristics(id),
    value VARCHAR
);

CREATE TABLE carts (
    id serial primary key,
    user_id int NOT NULL REFERENCES users(id),
    product_id int NOT NULL REFERENCES products(id),
    quantity INTEGER NOT NULL
);

CREATE TABLE favorites (
    id serial primary key,
    user_id int NOT NULL REFERENCES users(id),
    product_id int NOT NULL REFERENCES products(id)
);

CREATE TABLE reviews (
    id serial primary key,
    product_id int NOT NULL REFERENCES products(id),
    user_id int NOT NULL REFERENCES users(id),
    rating INTEGER,
    text TEXT,
    created_at TIMESTAMP
);

CREATE TABLE orders (
    id serial primary key,
    user_id int NOT NULL REFERENCES users(id),
    status VARCHAR NOT NULL,
    total DECIMAL NOT NULL,
    address_id int NOT NULL REFERENCES addresses(id),
    created_at TIMESTAMP,
    updated_at TIMESTAMP
);

CREATE TABLE order_items (
    id serial primary key,
    order_id int NOT NULL REFERENCES orders(id),
    product_id int NOT NULL REFERENCES products(id),
    quantity INTEGER NOT NULL,
    price DECIMAL NOT NULL
);

CREATE TABLE accruals (
    id serial primary key,
    wallet_id int NOT NULL REFERENCES wallets(id),
    order_id int REFERENCES orders(id),
    amount DECIMAL NOT NULL,
    commission DECIMAL,
    payout DECIMAL
);

CREATE TABLE payouts (
    id serial primary key,
    entrepreneur_id int NOT NULL REFERENCES entrepreneurs(id),
    amount DECIMAL NOT NULL,
    status VARCHAR,
    paid_at TIMESTAMP
);
CREATE TABLE supplies (
    id serial primary key,
    entrepreneur_id int NOT NULL REFERENCES entrepreneurs(id),
    status VARCHAR,
    created_at TIMESTAMP
);

CREATE TABLE supply_items (
    id serial primary key,
    supply_id int NOT NULL REFERENCES supplies(id),
    product_id int NOT NULL REFERENCES products(id),
    quantity INTEGER NOT NULL
);

INSERT INTO roles (name) VALUES ('Client');
INSERT INTO roles (name) VALUES ('Admin');