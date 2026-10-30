export interface JwtPayload {
  sub?: string;
  ssid?: string;
  exp?: number;
  [key: string]: unknown;
}

/**
 * 安全解析 JWT Token Payload
 */
export function extractJwtPayload(token: string): JwtPayload | null {
  if (!token || typeof token !== 'string') return null;
  const parts = token.trim().split('.');
  if (parts.length !== 3) return null;

  try {
    let payload = parts[1].replace(/-/g, '+').replace(/_/g, '/');
    while (payload.length % 4 !== 0) {
      payload += '=';
    }
    const decoded = atob(payload);
    return JSON.parse(decoded) as JwtPayload;
  } catch {
    return null;
  }
}

/**
 * 校验 Token 是否已经过期
 */
export function isTokenExpired(token: string): boolean {
  const payload = extractJwtPayload(token);
  if (!payload || !payload.exp) return false;
  const now = Math.floor(Date.now() / 1000);
  return now >= payload.exp;
}

/**
 * 校验 Token 是否即将过期 (默认提前 3600 秒 / 1小时)
 */
export function isTokenNearExpiry(token: string, bufferSeconds = 3600): boolean {
  const payload = extractJwtPayload(token);
  if (!payload || !payload.exp) return false;
  const now = Math.floor(Date.now() / 1000);
  return now + bufferSeconds >= payload.exp;
}
