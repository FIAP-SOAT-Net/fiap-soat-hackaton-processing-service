CREATE TABLE processing_files (
    id char(36) charset ascii not null primary key,
    file_id VARCHAR(36) NOT NULL,
    name VARCHAR(255) NOT NULL,
    bucket_name VARCHAR(255) NOT NULL,
    `key` VARCHAR(255) NOT NULL,
    file_size_bytes BIGINT UNSIGNED NOT NULL,
    mime_type VARCHAR(100) NULL,
    uploaded_at DATETIME(6) NOT NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'RECEIVED',
    created_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    updated_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
    KEY idx_processing_files_status (status),
    KEY idx_processing_files_uploaded_at (uploaded_at),
    KEY idx_processing_files_bucket_key (bucket_name, `key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE event_logs (
    id char(36) charset ascii not null primary key,
    processing_file_id CHAR(36) CHARSET ASCII NOT NULL,
    event_type VARCHAR(80) NOT NULL,
    status_from VARCHAR(30) NULL,
    status_to VARCHAR(30) NOT NULL,
    created_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    KEY idx_event_logs_file_id (processing_file_id),
    KEY idx_event_logs_event_type (event_type),
    KEY idx_event_logs_created_at (created_at),
    constraint fk_event_logs_processing_file_id foreign key (processing_file_id) references processing_files (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
