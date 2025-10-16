
import React from 'react';

export interface BannerProps {
	message: string;
	onClose: () => void;
    showModal: boolean;
    bannerType?: 'error' | 'warning' | 'success';
}

const Banner: React.FC<BannerProps> = ({ message, onClose, showModal, bannerType }) => {
	if (!message || !showModal) return null;

	let backgroundColor;
	let borderColor;
	let textColor;

	switch (bannerType) {
		case 'error':
			backgroundColor = '#ffdddd'; // red
			borderColor = '#ebccd1';
			textColor = '#a94442';
			break;
		case 'warning':
			backgroundColor = '#fffbe6'; // yellow
			borderColor = '#ffe58f';
			textColor = '#ad8b00';
			break;
		case 'success':
			backgroundColor = '#d4edda'; // green
			borderColor = '#c3e6cb';
			textColor = '#155724';
			break;
		default:
			backgroundColor = '#ffdddd';
			borderColor = '#ebccd1';
			textColor = '#a94442';
	}

	return (
		<div style={{
			background: backgroundColor,
			color: textColor,
			border: `1px solid ${borderColor}`,
			padding: '12px 20px',
			borderRadius: 4,
			marginBottom: 16,
			display: 'flex',
			alignItems: 'center',
			justifyContent: 'space-between',
			fontWeight: 500,
		}}>
			<span>{message}</span>
			{onClose && (
				<button onClick={onClose} style={{ marginLeft: 16, background: 'none', border: 'none', color: textColor, fontWeight: 'bold', cursor: 'pointer', fontSize: 18 }}>&times;</button>
			)}
		</div>
	);
};

export default React.memo(Banner);
