export * from './address.service';
import { AddressService } from './address.service';
export * from './admin.service';
import { AdminService } from './admin.service';
export * from './auth.service';
import { AuthService } from './auth.service';
export const APIS = [AddressService, AdminService, AuthService];
