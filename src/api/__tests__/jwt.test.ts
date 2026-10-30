import { describe, it, expect } from 'vitest';
import { extractJwtPayload, isTokenExpired, isTokenNearExpiry } from '../jwt';

describe('JWT Utility Functions', () => {
  // 构造模拟 JWT: header.payload.signature
  // payload: {"sub":"user_12345","ssid":"session_987","exp":2000000000}
  const futureExp = Math.floor(Date.now() / 1000) + 7200; // 2 小时后过期
  const payloadFuture = { sub: 'user_12345', ssid: 'session_987', exp: futureExp };
  const tokenFuture = `eyJhbGciOiJIUzI1NiJ9.${btoa(JSON.stringify(payloadFuture)).replace(/=/g, '')}.dummySignature`;

  const pastExp = Math.floor(Date.now() / 1000) - 300; // 5 分钟前已过期
  const payloadPast = { sub: 'user_expired', ssid: 'session_old', exp: pastExp };
  const tokenPast = `eyJhbGciOiJIUzI1NiJ9.${btoa(JSON.stringify(payloadPast)).replace(/=/g, '')}.dummySignature`;

  it('正确解析合法 JWT payload 字段', () => {
    const payload = extractJwtPayload(tokenFuture);
    expect(payload).not.toBeNull();
    expect(payload?.sub).toBe('user_12345');
    expect(payload?.ssid).toBe('session_987');
    expect(payload?.exp).toBe(futureExp);
  });

  it('非法格式返回 null', () => {
    expect(extractJwtPayload('invalid.token')).toBeNull();
    expect(extractJwtPayload('')).toBeNull();
  });

  it('准确判定过期状态', () => {
    expect(isTokenExpired(tokenFuture)).toBe(false);
    expect(isTokenExpired(tokenPast)).toBe(true);
  });

  it('准确判定即将过期状态 (1小时内)', () => {
    // 2 小时后过期 -> 1 小时内即将过期判定为 false
    expect(isTokenNearExpiry(tokenFuture, 3600)).toBe(false);
    
    // 30 分钟后过期 -> 1 小时内即将过期判定为 true
    const soonExp = Math.floor(Date.now() / 1000) + 1800;
    const tokenSoon = `eyJhbGciOiJIUzI1NiJ9.${btoa(JSON.stringify({ sub: 'u', exp: soonExp })).replace(/=/g, '')}.sig`;
    expect(isTokenNearExpiry(tokenSoon, 3600)).toBe(true);
  });
});
