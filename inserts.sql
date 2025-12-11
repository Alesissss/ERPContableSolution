INSERT INTO EMPRESA (nombre, razon_social, ruc)
VALUES ('Empresa Uno', 'Empresa Uno S.A.', '1234567890');

INSERT INTO AREA (codigo, nombre, empresa_id)
VALUES ('A001', 'Almacén', 1),
       ('A002', 'Compras', 1);

INSERT INTO DOCUMENTO (codigo, nombre)
VALUES ('OC', 'Orden de Compra'),
       ('RI', 'Requerimiento Interno'),
       ('IS', 'Ingreso/Salida Almacén');

INSERT INTO ESTADO (nombre, tabla)
VALUES ('Pendiente', 'REQINTERNO'),
       ('Aprobado', 'OCOMPRA'),
       ('Procesado', 'INGRESOSALIDAALM');

INSERT INTO PERSONAL (apellido_paterno, apellido_materno, nombres, dni, sexo, correo, telefono, username, password)
VALUES ('Gomez', 'Lopez', 'Juan', '12345678', 1, 'juan@mail.com', '987654321', 'j.gomez', 'pass123');


INSERT INTO PERSONAL_AREA (personal_id, area_id)
VALUES (1, 1),
       (1, 2);


INSERT INTO UMEDIDA (nombre, abreviatura)
VALUES ('Unidad', 'UND'),
       ('Kilogramo', 'KG');


INSERT INTO PRODUCTO (codigo, nombre, descripcion, stock, umedida_id)
VALUES ('P001', 'Laptop', 'Laptop 14 pulgadas', 10, 1),
       ('P002', 'Silla', 'Silla ergonómica', 20, 1);


INSERT INTO PROVEEDOR (nombre, razon_social, ruc)
VALUES ('Proveedor Uno', 'Proveedor Uno S.A.', '5555555555');


INSERT INTO REQINTERNO (fecha_hora, nota, personal_id, area_id, documento_id, estado_id)
VALUES ('2025-01-10 10:00:00', 'Solicitud de equipos', 1, 1, 2, 1);


INSERT INTO DREQINTERNO (reqinterno_id, producto_id, cantidad, observacion)
VALUES (1, 1, 2, 'Para oficina'),
       (1, 2, 5, 'Para almacén');


INSERT INTO OCOMPRA (fecha_hora, nota, reqinterno_id, documento_id, estado_id, proveedor_id)
VALUES ('2025-01-11 14:30:00', 'Compra aprobada', 1, 1, 2, 1);


INSERT INTO DCOMPRA (producto_id, ocompra_id, cantidad, precio, observacion)
VALUES (1, 1, 2, 3200.00, 'Laptop marca X'),
       (2, 1, 5, 150.00, 'Sillas reforzadas');


INSERT INTO INGRESOSALIDAALM (fecha_hora, nota, reqinterno_id, documento_id, ocompra_id, estado_id)
VALUES ('2025-01-12 09:00:00', 'Ingreso de productos comprados', 1, 3, 1, 3);


INSERT INTO DINGRESOSALIDAALM (ingresosalidaalm_id, producto_id, cantidad, observacion)
VALUES (1, 1, 2, 'Ingreso a almacén'),
       (1, 2, 5, 'Ingreso a almacén');


