CREATE TABLE files (
    id CHAR(36) NOT NULL,
    name VARCHAR(255) NOT NULL,
    bucket_name VARCHAR(255) NOT NULL,
    `key` VARCHAR(1024) NOT NULL,
    file_size_bytes BIGINT UNSIGNED NOT NULL,
    mime_type VARCHAR(100) NULL,
    uploaded_at DATETIME(6) NOT NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'RECEIVED',
    created_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    updated_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
    PRIMARY KEY (id),
    KEY idx_files_status (status),
    KEY idx_files_uploaded_at (uploaded_at),
    KEY idx_files_bucket_key (bucket_name, `key`),
    CONSTRAINT chk_files_status
        CHECK (status IN ('RECEIVED', 'PROCESSING', 'ANALYZED', 'ERROR', 'PROCESSED'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE event_logs (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    file_id CHAR(36) NOT NULL,
    event_type VARCHAR(80) NOT NULL,
    status_from VARCHAR(30) NULL,
    status_to VARCHAR(30) NOT NULL,
    created_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    PRIMARY KEY (id),
    KEY idx_event_logs_file_id (file_id),
    KEY idx_event_logs_event_type (event_type),
    KEY idx_event_logs_created_at (created_at),
    CONSTRAINT chk_event_logs_status_from
        CHECK (status_from IS NULL OR status_from IN ('RECEIVED', 'PROCESSING', 'ANALYZED', 'ERROR', 'PROCESSED')),
    CONSTRAINT chk_event_logs_status_to
        CHECK (status_to IN ('RECEIVED', 'PROCESSING', 'ANALYZED', 'ERROR', 'PROCESSED')),
    CONSTRAINT fk_event_logs_file_id
        FOREIGN KEY (file_id) REFERENCES files (id)
        ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;