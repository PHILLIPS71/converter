/**
 * Converts a string into a hexadecimal color code.
 * @param text - Input string to convert
 * @returns A hexadecimal color code (e.g., '#ff0000')
 */
export const getHashedColor = (text: string): string => {
  // generate hash using bitwise operations
  const hash = text.split('').reduce((acc, char) => {
    const value = char.charCodeAt(0)
    return (acc << 5) - acc + value
  }, 0)

  // convert hash to hex color
  const colorComponents = Array.from({ length: 3 }, (_, i) => {
    const value = (hash >> (i * 8)) & 0xff
    return value.toString(16).padStart(2, '0')
  })

  return `#${colorComponents.join('')}`
}
