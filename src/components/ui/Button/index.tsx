import { Loader2 } from 'lucide-react'
import { ButtonHTMLAttributes, FC } from 'react'

const getBtnVariant = (variant: string) => ({
  normal: 'bg-slate-900 text-white hover:bg-slate-800',
  ghost: 'bg-transparent hover:text-slate-900 hover:bg-slate-200',
})[variant]

const getBtnSize = (size: string) => ({
  sm: 'h-9 px-2',
  md: 'h-10 py-2 px-4',
  lg: 'h-11 px-8',
})[size]

const getBtnClassName = (variant?: string, size?: string, className?: string) => {
  const classVariant = getBtnVariant(variant ?? 'normal')
  const classSize = getBtnSize(size ?? 'md')
  const others = className ?? ''
  
  const btnClassName = `
    active:scale-95 inline-flex items-center justify-center rounded-md text-sm font-medium
    transition-color focus:outline-none focus:ring-2 focus:ring-slate-400 focus:ring-offset-2
    disabled:opacity-50 disabled:pointer-events-none ${classVariant} ${classSize} ${others}
  `

  return btnClassName.replace(/\s+/g, ' ').trim()
}

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  isLoading?: boolean
  variant?: 'normal' | 'ghost'
  size?: 'md' | 'sm' | 'lg'
  type?: 'button' | 'submit' | 'reset'
  className?: string
}

export const Button: FC<ButtonProps> = ({ children, isLoading, variant, size, type, className, ...props }) => {
  const btnClassName = getBtnClassName(variant, size, className)

  return (
    <button
      {...props}
      className={btnClassName}
      type={type ?? 'button'}
      disabled={!!isLoading}
    >
      {!!isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
      {children}
    </button>
  )
}
